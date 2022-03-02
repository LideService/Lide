using System;
using System.Collections.Concurrent;
using System.IO;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.Core.Model.Settings;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public sealed class SubstituteRecordDecorator : IObjectDecoratorReadonly, IDisposable
    {
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly IPropagateContentHandler _propagateContentHandler;
        private readonly ISignatureProvider _signatureProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IStreamBatchProvider _streamBatchProvider;
        private readonly ITaskRunner _taskRunner;
        private readonly Stream _fileStream;
        private readonly bool _enabled;

        public SubstituteRecordDecorator(
            IBinarySerializeProvider binarySerializeProvider,
            IPropagateContentHandler propagateContentHandler,
            ISignatureProvider signatureProvider,
            ISettingsProvider settingsProvider,
            IStreamBatchProvider streamBatchProvider,
            IPathFacade pathFacade,
            IFileFacade fileFacade,
            ITaskRunner taskRunner)
        {
            _binarySerializeProvider = binarySerializeProvider;
            _propagateContentHandler = propagateContentHandler;
            _signatureProvider = signatureProvider;
            _settingsProvider = settingsProvider;
            _streamBatchProvider = streamBatchProvider;
            _taskRunner = taskRunner;

            _enabled = settingsProvider.IsDecoratorIncluded(Id);
            if (_enabled)
            {
                var filePath = pathFacade.Combine(pathFacade.GetTempPath(), fileFacade.GetFileName(Id));
                _fileStream = fileFacade.OpenFile(filePath);
            }

            _propagateContentHandler.ParseOwnRequest += ParseOwnRequest;
            _propagateContentHandler.ParseOutgoingResponse += ParseOutgoingResponse;
            _propagateContentHandler.PrepareOwnResponse += PrepareOwnResponse;
        }

        public string Id => "Lide.Substitute.Record";

        public void Dispose()
        {
            _propagateContentHandler.ParseOwnRequest -= ParseOwnRequest;
            _propagateContentHandler.ParseOutgoingResponse -= ParseOutgoingResponse;
            _propagateContentHandler.PrepareOwnResponse -= PrepareOwnResponse;
            _fileStream?.Close();
            _fileStream?.Dispose();
        }

        public void ExecuteBeforeInvoke(MethodMetadata methodMetadata)
        {
            var methodSignature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.AllSet);
            var inputParameters = _binarySerializeProvider.Serialize(methodMetadata.ParametersMetadata.GetOriginalParameters());
            var before = new SubstituteMethodBefore
            {
                CallId = methodMetadata.CallId,
                MethodSignature = methodSignature,
                InputParameters = inputParameters,
            };

            var serialized = _binarySerializeProvider.Serialize(before);
            _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));
        }

        public void ExecuteAfterResult(MethodMetadata methodMetadata)
        {
            var result = methodMetadata.ReturnMetadata.GetOriginalException() ?? methodMetadata.ReturnMetadata.GetOriginalResult();
            var inputParameters = _binarySerializeProvider.Serialize(methodMetadata.ParametersMetadata.GetOriginalParameters());
            var after = new SubstituteMethodAfter
            {
                CallId = methodMetadata.CallId,
                IsException = methodMetadata.ReturnMetadata.GetOriginalException() != null,
                OutputData = _binarySerializeProvider.Serialize(result),
                InputParameters = inputParameters,
            };

            var serialized = _binarySerializeProvider.Serialize(after);
            _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));
        }

        private void ParseOwnRequest(ConcurrentDictionary<string, byte[]> content)
        {
            if (!_enabled)
            {
                return;
            }

            content.TryGetValue(PropagateProperties.RequestContent, out var requestContent);
            var request = new SubstituteOwnRequest
            {
                Path = _settingsProvider.OriginRequestPath,
                Content = requestContent,
            };
            var serialized = _binarySerializeProvider.Serialize(request);
            _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));
        }

        private void ParseOutgoingResponse(ConcurrentDictionary<string, byte[]> content, string path, long requestId, Exception exception)
        {
            if (!_enabled)
            {
                return;
            }

            content.TryGetValue(PropagateProperties.SubstituteContent, out var childContent);
            var child = new SubstituteOutgoingResponse()
            {
                Path = path,
                RequestId = requestId,
                Content = childContent,
                Exception = exception,
            };

            var serialized = _binarySerializeProvider.Serialize(child);
            _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));
        }

        private void PrepareOwnResponse(ConcurrentDictionary<string, byte[]> container)
        {
            if (!_enabled)
            {
                return;
            }

            _taskRunner.WaitQueue().Wait();
            using var reader = new MemoryStream();
            _fileStream.CopyTo(reader);
            var content = reader.ToArray();
            container[PropagateProperties.SubstituteContent] = content;
        }
    }
}