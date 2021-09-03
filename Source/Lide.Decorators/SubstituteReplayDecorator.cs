using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class SubstituteReplayDecorator : IObjectDecoratorVolatile
    {
        private readonly IParametersSerializer _parametersSerializer;
        private readonly IFileFacade _fileFacade;
        private readonly ICompressionProvider _compressionProvider;
        private readonly ISerializerFacade _serializerFacade;
        private readonly ISignatureProvider _signatureProvider;
        private readonly List<SubstituteBefore> _befores;
        private readonly Dictionary<int, SubstituteAfter> _afters;
        private readonly Dictionary<int, int> _ids;
        private bool _loaded;

        public SubstituteReplayDecorator(
            IFileFacade fileFacade,
            ICompressionProvider compressionProvider,
            ISerializerFacade serializerFacade,
            ISignatureProvider signatureProvider,
            IParametersSerializer parametersSerializer)
        {
            _fileFacade = fileFacade;
            _compressionProvider = compressionProvider;
            _serializerFacade = serializerFacade;
            _signatureProvider = signatureProvider;
            _parametersSerializer = parametersSerializer;
            _befores = new List<SubstituteBefore>();
            _afters = new Dictionary<int, SubstituteAfter>();
            _ids = new Dictionary<int, int>();
        }

        public string Id => "Lide.Substitute.Replay";

        public void ExecuteBeforeInvoke(MethodMetadataVolatile methodMetadata)
        {
            Load();
            var methodSignature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.OnlyBaseNamespace);
            var callerSignature = _signatureProvider.GetCallerSignature();
            var input = _befores.First(x => x.MethodSignature == methodSignature && x.CallerSignature == callerSignature);
            _befores.Remove(input);

            var parameters = _parametersSerializer.Deserialize(input.InputParameters);
            var methodHash = methodMetadata.MethodInfo.GetHashCode();
            var parametersHash = parameters.GetHashCode();
            var signatureHash = HashCode.Combine(methodHash, parametersHash);
            _ids.TryAdd(signatureHash, input.Id);

            methodMetadata.ParametersMetadataVolatile.SetParameters(parameters);
        }

        public void ExecuteAfterResult(MethodMetadataVolatile methodMetadata)
        {
            Load();
            var methodHash = methodMetadata.MethodInfo.GetHashCode();
            var parametersHash = methodMetadata.ParametersMetadataVolatile.GetEditedParameters().GetHashCode();
            var signatureHash = HashCode.Combine(methodHash, parametersHash);
            var id = _ids[signatureHash];
            var after = _afters[id];
            var result = _parametersSerializer.DeserializeSingle(after.Data);
            if (after.IsException)
            {
                methodMetadata.ReturnMetadataVolatile.SetException((Exception)result);
            }
            else
            {
                methodMetadata.ReturnMetadataVolatile.SetResult(result);
            }
        }

        private void Load()
        {
            if (_loaded)
            {
                return;
            }

            _loaded = true;
            var filePath = "/home/nikola.gamzakov/substitute";
            (byte[] data, int endPosition) nextBatch = (null, 0);
            do
            {
                nextBatch = _fileFacade.ReadNextBatch(filePath, nextBatch.endPosition).Result;
                if (nextBatch.endPosition == -1)
                {
                    break;
                }

                var decompressed = _compressionProvider.Decompress(nextBatch.data);
                var type = BitConverter.ToInt32(decompressed.Take(4).ToArray());
                var data = decompressed.Skip(4).ToArray();
                if (type == 0)
                {
                    var input = _serializerFacade.Deserialize<SubstituteBefore>(data);
                    _befores.Add(input);
                }

                if (type == 1)
                {
                    var result = _serializerFacade.Deserialize<SubstituteAfter>(data);
                    _afters.TryAdd(result.Id, result);
                }
            }
            while (nextBatch.endPosition != -1);
        }
    }
}