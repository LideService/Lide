using System;
using System.Threading.Tasks;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class SubstituteRecordDecorator : IObjectDecoratorReadonly
    {
        private static readonly byte[] BeforeBytes = BitConverter.GetBytes((int)SubstituteType.Before);
        private static readonly byte[] AfterBytes = BitConverter.GetBytes((int)SubstituteType.After);

        private readonly ICompressionProvider _compressionProvider;
        private readonly ISignatureProvider _signatureProvider;
        private readonly IParametersSerializer _parametersSerializer;
        private readonly ISerializerFacade _serializerFacade;
        private readonly IFileFacade _fileFacade;
        private readonly ITaskRunner _taskRunner;
        private readonly string _filePath;

        public SubstituteRecordDecorator(
            ICompressionProvider compressionProvider,
            ISignatureProvider signatureProvider,
            IParametersSerializer parametersSerializer,
            ISerializerFacade serializerFacade,
            IFileFacade fileFacade,
            ITaskRunner taskRunner)
        {
            _compressionProvider = compressionProvider;
            _signatureProvider = signatureProvider;
            _parametersSerializer = parametersSerializer;
            _serializerFacade = serializerFacade;
            _fileFacade = fileFacade;
            _taskRunner = taskRunner;

            _filePath = "/home/nikola.gamzakov/substitute";
            ////_fileFacade.DeleteFile(_filePath);
        }

        public string Id { get; } = "Lide.Substitute.Record";

        public void ExecuteBeforeInvoke(MethodMetadata methodMetadata)
        {
            var methodSignature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.AllSet);
            var inputParameters = _parametersSerializer.Serialize(methodMetadata.ParametersMetadata.GetOriginalParameters());
            var before = new SubstituteBefore
            {
                CallId = methodMetadata.CallId,
                MethodSignature = methodSignature,
                InputParameters = inputParameters,
            };

            _taskRunner.AddToQueue(new Task<Task>(async () =>
            {
                var data = _serializerFacade.Serialize(before);
                var compressed = _compressionProvider.Compress(data);
                await _fileFacade.WriteToFile(_filePath, BeforeBytes).ConfigureAwait(false);
                await _fileFacade.WriteToFile(_filePath, compressed).ConfigureAwait(false);
            }));
        }

        public void ExecuteAfterResult(MethodMetadata methodMetadata)
        {
            var result = methodMetadata.ReturnMetadata.GetOriginalException() ?? methodMetadata.ReturnMetadata.GetOriginalResult();
            var inputParameters = _parametersSerializer.Serialize(methodMetadata.ParametersMetadata.GetOriginalParameters());
            var after = new SubstituteAfter()
            {
                CallId = methodMetadata.CallId,
                IsException = methodMetadata.ReturnMetadata.GetOriginalException() != null,
                OutputData = _parametersSerializer.SerializeSingle(result),
                InputParameters = inputParameters,
            };

            _taskRunner.AddToQueue(new Task<Task>(async () =>
            {
                var data = _serializerFacade.Serialize(after);
                var compressed = _compressionProvider.Compress(data);
                await _fileFacade.WriteToFile(_filePath, AfterBytes).ConfigureAwait(false);
                await _fileFacade.WriteToFile(_filePath, compressed).ConfigureAwait(false);
            }));
        }
    }
}