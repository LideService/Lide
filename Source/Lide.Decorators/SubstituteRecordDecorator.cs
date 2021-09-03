using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lide.Core.Contract;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class SubstituteRecordDecorator : IObjectDecoratorReadonly
    {
        private readonly ICompressionProvider _compressionProvider;
        private readonly ISignatureProvider _signatureProvider;
        private readonly IParametersSerializer _parametersSerializer;
        private readonly ISerializerFacade _serializerFacade;
        private readonly IFileFacade _fileFacade;
        private readonly string _filePath;
        private readonly ConcurrentDictionary<int, int> _ids;
        private int _id;

        public SubstituteRecordDecorator(
            ICompressionProvider compressionProvider,
            ISignatureProvider signatureProvider,
            IParametersSerializer parametersSerializer,
            ISerializerFacade serializerFacade,
            IFileFacade fileFacade,
            IScopeIdProvider scopeIdProvider,
            IPathFacade pathFacade,
            IDateTimeFacade dateTimeFacade)
        {
            _compressionProvider = compressionProvider;
            _signatureProvider = signatureProvider;
            _parametersSerializer = parametersSerializer;
            _serializerFacade = serializerFacade;
            _fileFacade = fileFacade;

            var tmpPath = pathFacade.GetTempPath();
            var date = dateTimeFacade.GetDateNow().ToString("yyyyMMddHHmmss");
            ////var fileName = $"{scopeIdProvider.GetRootScopeId()}.{scopeIdProvider.GetCurrentScopeId()}.Lide.Substitute.{date}";
            var fileName = "/home/nikola.gamzakov/substitute";
            _filePath = pathFacade.Combine(tmpPath, fileName);
            _ids = new ConcurrentDictionary<int, int>();
        }

        public string Id { get; } = "Lide.Substitute.Record";

        public void ExecuteBeforeInvoke(MethodMetadata methodMetadata)
        {
            var methodSignature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.OnlyBaseNamespace);
            var callerSignature = _signatureProvider.GetCallerSignature();

            var nextId = Interlocked.Increment(ref _id);
            var methodHash = methodMetadata.MethodInfo.GetHashCode();
            var parametersHash = methodMetadata.ParametersMetadata.GetOriginalParameters().GetHashCode();
            var signatureHash = HashCode.Combine(methodHash, parametersHash);
            _ids.TryAdd(signatureHash, nextId);

            var inputParameters = _parametersSerializer.Serialize(methodMetadata.ParametersMetadata.GetOriginalParameters());
            var before = new SubstituteBefore
            {
                Id = nextId,
                MethodSignature = methodSignature,
                CallerSignature = callerSignature,
                InputParameters = inputParameters,
            };

            var data = _serializerFacade.Serialize(before);
            var dataExtended = BitConverter.GetBytes(0).Concat(data).ToArray();
            var compressed = _compressionProvider.Compress(dataExtended);
            _fileFacade.WriteToFile(_filePath, compressed).Wait();
        }

        public void ExecuteAfterResult(MethodMetadata methodMetadata)
        {
            var methodHash = methodMetadata.MethodInfo.GetHashCode();
            var parametersHash = methodMetadata.ParametersMetadata.GetOriginalParameters().GetHashCode();
            var signatureHash = HashCode.Combine(methodHash, parametersHash);

            var id = _ids[signatureHash];
            var result = methodMetadata.ReturnMetadata.GetOriginalException() ?? methodMetadata.ReturnMetadata.GetOriginalResult();
            var after = new SubstituteAfter()
            {
                Id = id,
                IsException = methodMetadata.ReturnMetadata.GetOriginalException() != null,
                Data = _parametersSerializer.SerializeSingle(result),
            };

            var data = _serializerFacade.Serialize(after);
            var dataExtended = BitConverter.GetBytes(1).Concat(data).ToArray();
            var compressed = _compressionProvider.Compress(dataExtended);
            _fileFacade.WriteToFile(_filePath, compressed).Wait();
        }
    }
}