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
        private readonly ISignatureProvider _signatureProvider;
        private readonly IStreamBatchProvider _streamBatchProvider;
        private readonly ITaskRunner _taskRunner;
        private readonly Stream _fileStream;

        public SubstituteRecordDecorator(
            IBinarySerializeProvider binarySerializeProvider,
            ISignatureProvider signatureProvider,
            IStreamBatchProvider streamBatchProvider,
            IPathFacade pathFacade,
            IFileFacade fileFacade,
            ITaskRunner taskRunner)
        {
            _binarySerializeProvider = binarySerializeProvider;
            _signatureProvider = signatureProvider;
            _streamBatchProvider = streamBatchProvider;
            _taskRunner = taskRunner;

            var filePath = pathFacade.Combine(pathFacade.GetTempPath(), fileFacade.GetFileName(Id));
            _fileStream = fileFacade.OpenFile(filePath);
        }

        public string Id => "Lide.Substitute.Record";

        public void Dispose()
        {
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
                SerializedObject = methodMetadata.IsSingleton
                    ? _binarySerializeProvider.Serialize(methodMetadata.PlainObject)
                    : Array.Empty<byte>(),
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
                SerializedObject = methodMetadata.IsSingleton
                    ? _binarySerializeProvider.Serialize(methodMetadata.PlainObject)
                    : Array.Empty<byte>(),
            };

            var serialized = _binarySerializeProvider.Serialize(after);
            _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));
        }

        public void ParseOwnRequest(ConcurrentDictionary<string, byte[]> content)
        {
            content.TryGetValue(PropagateProperties.OriginalContent, out var requestContent);
            content.TryGetValue(PropagateProperties.OriginalQuery, out var queryContent);
            var request = new SubstituteOwnContent
            {
                TestId = 81,
                Content = requestContent ?? Array.Empty<byte>(),
                Query = queryContent ?? Array.Empty<byte>(),
            };
            var serialized = _binarySerializeProvider.Serialize(request);
            _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));
        }

        public void ParseOutgoingResponse(ConcurrentDictionary<string, byte[]> content, string path, long requestId, Exception exception)
        {
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

        public void PrepareOwnResponse(ConcurrentDictionary<string, byte[]> container)
        {
            container.TryGetValue(PropagateProperties.OriginalContent, out var responseContent);
            var response = new SubstituteOwnContent
            {
                TestId = 63,
                Content = responseContent ?? Array.Empty<byte>(),
                Query = Array.Empty<byte>(),
            };

            var serialized = _binarySerializeProvider.Serialize(response);
            _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));

            _taskRunner.WaitQueue().Wait();
            using var reader = new MemoryStream();
            _fileStream.Seek(0, SeekOrigin.Begin);
            _fileStream.CopyTo(reader);
            var content = reader.ToArray();
            container[PropagateProperties.SubstituteContent] = content;
        }
    }
}