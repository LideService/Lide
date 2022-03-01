using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private readonly object _lockObject = new ();
        private bool _initialized;

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

            var filePath = pathFacade.Combine(pathFacade.GetTempPath(), fileFacade.GetFileName(Id));
            _fileStream = fileFacade.OpenFile(filePath);
            _propagateContentHandler.PrepareForParent += PropagateContentHandlerOnPrepareForParent;
            _propagateContentHandler.ParseFromChild += PropagateContentHandlerOnParseFromChild;
        }

        public string Id => "Lide.Substitute.Record";

        public void Dispose()
        {
            _propagateContentHandler.PrepareForParent -= PropagateContentHandlerOnPrepareForParent;
            _propagateContentHandler.ParseFromChild -= PropagateContentHandlerOnParseFromChild;
            _fileStream.Close();
            _fileStream.Dispose();
        }

        public void ExecuteBeforeInvoke(MethodMetadata methodMetadata)
        {
            ExecuteInitialization();
            var methodSignature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.AllSet);
            var inputParameters = _binarySerializeProvider.Serialize(methodMetadata.ParametersMetadata.GetOriginalParameters());
            var before = new SubstituteBefore
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
            ExecuteInitialization();
            var result = methodMetadata.ReturnMetadata.GetOriginalException() ?? methodMetadata.ReturnMetadata.GetOriginalResult();
            var inputParameters = _binarySerializeProvider.Serialize(methodMetadata.ParametersMetadata.GetOriginalParameters());
            var after = new SubstituteAfter
            {
                CallId = methodMetadata.CallId,
                IsException = methodMetadata.ReturnMetadata.GetOriginalException() != null,
                OutputData = _binarySerializeProvider.Serialize(result),
                InputParameters = inputParameters,
            };

            var serialized = _binarySerializeProvider.Serialize(after);
            _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));
        }

        private void ExecuteInitialization()
        {
            if (!_initialized)
            {
                lock (_lockObject)
                {
                    if (!_initialized)
                    {
                        _initialized = true;
                        _propagateContentHandler.ParentData.TryGetValue(PropagateProperties.OriginalRequest, out var originalContent);
                        _propagateContentHandler.ParentData.TryGetValue(PropagateProperties.OriginalHeaders, out var originalHeaders);
                        var request = new SubstituteRequest
                        {
                            Path = _settingsProvider.OriginRequestPath,
                            OriginalContent = originalContent,
                            OriginalHeaders = originalHeaders,
                        };
                        var serialized = _binarySerializeProvider.Serialize(request);
                        _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));
                    }
                }
            }
        }

        private void PropagateContentHandlerOnPrepareForParent(ConcurrentDictionary<string, byte[]> container, string requestPath)
        {
            _taskRunner.WaitQueue().Wait();
            using var reader = new MemoryStream();
            _fileStream.CopyTo(reader);
            var content = reader.ToArray();
            container["SubstituteContent"] = content;
        }

        private void PropagateContentHandlerOnParseFromChild(Dictionary<string, byte[]> content, Exception e, string requestPath)
        {
            var childContent = content["SubstituteContent"];
            var child = new SubstituteChild()
            {
                Path = requestPath,
                Content = childContent,
                Exception = e,
            };

            var serialized = _binarySerializeProvider.Serialize(child);
            _taskRunner.AddToQueue(() => _streamBatchProvider.WriteNextBatch(_fileStream, serialized));
        }
    }
}