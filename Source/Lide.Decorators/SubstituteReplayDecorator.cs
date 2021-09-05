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
        private readonly Dictionary<long, SubstituteAfter> _afters;
        private readonly Dictionary<long, long> _ids;
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
            _afters = new Dictionary<long, SubstituteAfter>();
            _ids = new Dictionary<long, long>();
        }

        public string Id => "Lide.Substitute.Replay";

        public void ExecuteBeforeInvoke(MethodMetadataVolatile methodMetadata)
        {
            Load();
            var methodSignature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.AllSet);
            var before = _befores.First(x => x.MethodSignature == methodSignature);
            _befores.Remove(before);

            var parameters = _parametersSerializer.Deserialize(before.InputParameters);
            _ids[methodMetadata.CallId] = before.CallId;

            methodMetadata.ParametersMetadataVolatile.SetParameters(parameters);
        }

        public void ExecuteAfterResult(MethodMetadataVolatile methodMetadata)
        {
            Load();
            var id = _ids[methodMetadata.CallId];
            var after = _afters[id];
            var result = _parametersSerializer.DeserializeSingle(after.OutputData);
            var input = _parametersSerializer.Deserialize(after.InputParameters);
            if (after.IsException)
            {
                methodMetadata.ReturnMetadataVolatile.SetException((Exception)result);
            }
            else
            {
                methodMetadata.ReturnMetadataVolatile.SetResult(result);
            }

            // Must no be done like this, but .. eehhh whatever
            var objects = methodMetadata.ParametersMetadataVolatile.GetOriginalParameters();
            for (var index = 0; index < objects.Length; index++)
            {
                if (input[index].GetType().IsValueType)
                {
                    input[index] = objects[index];
                    continue;
                }

                _serializerFacade.PopulateObject(input[index], objects[index]);
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
            var nextBatch = new BinaryFileBatch();
            while (true)
            {
                nextBatch = _fileFacade.ReadNextBatch(filePath, nextBatch.EndPosition).Result;
                var typeBytes = nextBatch.Data;
                if (nextBatch.EndPosition == -1)
                {
                    break;
                }

                nextBatch = _fileFacade.ReadNextBatch(filePath, nextBatch.EndPosition).Result;
                var dataBytes = nextBatch.Data;
                var type = (SubstituteType)BitConverter.ToInt32(typeBytes);
                var data = _compressionProvider.Decompress(dataBytes);
                switch (type)
                {
                    case SubstituteType.Before:
                        _befores.Add(_serializerFacade.Deserialize<SubstituteBefore>(data));
                        break;
                    case SubstituteType.After:
                        var result = _serializerFacade.Deserialize<SubstituteAfter>(data);
                        _afters.TryAdd(result.CallId, result);
                        break;
                }
            }
        }
    }
}