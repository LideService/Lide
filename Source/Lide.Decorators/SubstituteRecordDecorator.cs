using System;
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
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly IFileFacade _fileFacade;
        private readonly ITaskRunner _taskRunner;
        private readonly string _filePath;

        public SubstituteRecordDecorator(
            ICompressionProvider compressionProvider,
            ISignatureProvider signatureProvider,
            IBinarySerializeProvider binarySerializeProvider,
            IFileFacade fileFacade,
            IPathProvider pathProvider,
            ITaskRunner taskRunner)
        {
            _compressionProvider = compressionProvider;
            _signatureProvider = signatureProvider;
            _binarySerializeProvider = binarySerializeProvider;
            _fileFacade = fileFacade;
            _taskRunner = taskRunner;

            _filePath = pathProvider.GetDecoratorFilePath(Id, true);
            _filePath = "/home/nikola.gamzakov/substitute";
            _fileFacade.DeleteFile(_filePath);
        }

        public string Id => "Lide.Substitute.Record";

        public void ExecuteBeforeInvoke(MethodMetadata methodMetadata)
        {
            var methodSignature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.AllSet);
            var inputParameters = _binarySerializeProvider.Serialize(methodMetadata.ParametersMetadata.GetOriginalParameters());
            var before = new SubstituteBefore
            {
                CallId = methodMetadata.CallId,
                MethodSignature = methodSignature,
                InputParameters = inputParameters,
            };
            var data = _binarySerializeProvider.Serialize(before);
            var compressed = _compressionProvider.Compress(data);
            _taskRunner.AddToQueue(_fileFacade.WriteNextBatch(_filePath, BeforeBytes));
            _taskRunner.AddToQueue(_fileFacade.WriteNextBatch(_filePath, compressed));
        }

        public void ExecuteAfterResult(MethodMetadata methodMetadata)
        {
            var result = methodMetadata.ReturnMetadata.GetOriginalException() ?? methodMetadata.ReturnMetadata.GetOriginalResult();
            var inputParameters = _binarySerializeProvider.Serialize(methodMetadata.ParametersMetadata.GetOriginalParameters());
            var after = new SubstituteAfter()
            {
                CallId = methodMetadata.CallId,
                IsException = methodMetadata.ReturnMetadata.GetOriginalException() != null,
                OutputData = _binarySerializeProvider.Serialize(result),
                InputParameters = inputParameters,
            };

            var data = _binarySerializeProvider.Serialize(after);
            var compressed = _compressionProvider.Compress(data);
            _taskRunner.AddToQueue(_fileFacade.WriteNextBatch(_filePath, AfterBytes));
            _taskRunner.AddToQueue(_fileFacade.WriteNextBatch(_filePath, compressed));
        }
    }
}