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
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly IPropagateContentHandler _propagateContentHandler;
        private readonly ISignatureProvider _signatureProvider;
        private readonly SubstituteParser _substituteParser;
        private readonly ConcurrentDictionary<long, long> _callIds = new ();
        private readonly bool _enabled;

        public SubstituteReplayDecorator(
            ISettingsProvider settingsProvider,
            IBinarySerializeProvider binarySerializeProvider,
            IPropagateContentHandler propagateContentHandler,
            IStreamBatchProvider streamBatchProvider,
            ISignatureProvider signatureProvider)
        {
            _enabled = settingsProvider.IsDecoratorIncluded(Id);
            _binarySerializeProvider = binarySerializeProvider;
            _propagateContentHandler = propagateContentHandler;
            _signatureProvider = signatureProvider;
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
        }

        private void ParseOwnRequest(ConcurrentDictionary<string, byte[]> content)
        {
            if (!_enabled)
            {
                return;
            }

            content.TryGetValue(PropagateProperties.SubstituteContent, out var requestContent);
            var ownRequest = _binarySerializeProvider.Deserialize<SubstituteOwnRequest>(requestContent);
            var contentStream = new MemoryStream(ownRequest.Content);
            _substituteParser.LoadAll(contentStream);
        }

        private void PrepareOutgoingRequest(ConcurrentDictionary<string, byte[]> container, string path, long requestId, byte[] content)
        {
            var innerContent = _substituteParser.GetOutgoingResponse(path);
            if (innerContent == null)
            {
                return;
            }

            container[PropagateProperties.SubstituteContent] = innerContent.Content;
        }
    }
}