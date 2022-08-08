using System;
using System.Collections.Concurrent;
using System.IO;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.Core.Model.Settings;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public sealed class SubstituteReplayDecorator : IObjectDecoratorVolatile, IDisposable
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly IPropagateContentHandler _propagateContentHandler;
        private readonly ISignatureProvider _signatureProvider;
        private readonly IActivatorProvider _activatorProvider;
        private readonly SubstituteParser _substituteParser;
        private readonly ConcurrentDictionary<long, long> _callIds = new ();

        public SubstituteReplayDecorator(
            ISettingsProvider settingsProvider,
            IBinarySerializeProvider binarySerializeProvider,
            IPropagateContentHandler propagateContentHandler,
            IStreamBatchProvider streamBatchProvider,
            ISignatureProvider signatureProvider,
            IActivatorProvider activatorProvider)
        {
            _settingsProvider = settingsProvider;
            _binarySerializeProvider = binarySerializeProvider;
            _propagateContentHandler = propagateContentHandler;
            _signatureProvider = signatureProvider;
            _activatorProvider = activatorProvider;
            _substituteParser = new SubstituteParser(binarySerializeProvider, streamBatchProvider);

            _propagateContentHandler.ParseOwnRequest += ParseOwnRequest;
            _propagateContentHandler.PrepareOutgoingRequest += PrepareOutgoingRequest;
        }

        public string Id => "Lide.Substitute.Replay";

        public void Dispose()
        {
            _propagateContentHandler.ParseOwnRequest -= ParseOwnRequest;
            _propagateContentHandler.PrepareOutgoingRequest -= PrepareOutgoingRequest;
        }

        public void ExecuteBeforeInvoke(MethodMetadataVolatile methodMetadata)
        {
            var signature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.AllSet);
            var before = _substituteParser.GetBeforeMethod(signature);
            if (before == null)
            {
                return;
            }

            _callIds[methodMetadata.CallId] = before.CallId;
            var parameters = (object[])_binarySerializeProvider.Deserialize(before.InputParameters);
            methodMetadata.ParametersMetadataVolatile.SetParameters(parameters);
            if (methodMetadata.IsSingleton)
            {
                var deserializedObject = _binarySerializeProvider.Deserialize(before.SerializedObject);
                _activatorProvider.DeepCopyIntoExistingObject(deserializedObject, methodMetadata.PlainObject);
            }
        }

        public void ExecuteAfterResult(MethodMetadataVolatile methodMetadata)
        {
            _callIds.TryGetValue(methodMetadata.CallId, out var callId);
            var after = _substituteParser.GetAfterMethod(callId);
            if (after == null)
            {
                return;
            }

            var result = _binarySerializeProvider.Deserialize(after.OutputData);
            if (after.IsException)
            {
                methodMetadata.ReturnMetadataVolatile.SetException((Exception)result);
            }
            else
            {
                methodMetadata.ReturnMetadataVolatile.SetResult(result);
            }

            if (methodMetadata.IsSingleton)
            {
                var deserializedObject = _binarySerializeProvider.Deserialize(after.SerializedObject);
                _activatorProvider.DeepCopyIntoExistingObject(deserializedObject, methodMetadata.PlainObject);
            }
        }

        private void ParseOwnRequest(ConcurrentDictionary<string, byte[]> content)
        {
            if (!_settingsProvider.IsDecoratorIncluded(Id))
            {
                return;
            }

            content.TryGetValue(PropagateProperties.SubstituteContent, out var requestContent);
            var contentStream = new MemoryStream(requestContent);
            _substituteParser.LoadAll(contentStream);

            content[PropagateProperties.OriginalContent] = _substituteParser.SubstituteOwnRequest.Content;
            content[PropagateProperties.OriginalQuery] = _substituteParser.SubstituteOwnRequest.Query;
        }

        private void PrepareOutgoingRequest(ConcurrentDictionary<string, byte[]> container, string path, long requestId, byte[] content)
        {
            if (!_settingsProvider.IsDecoratorIncluded(Id))
            {
                return;
            }

            var innerContent = _substituteParser.GetOutgoingResponse(path);
            if (innerContent == null)
            {
                return;
            }

            container[PropagateProperties.SubstituteContent] = innerContent.Content;
        }
    }
}