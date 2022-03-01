using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class SubstituteReplayDecorator : IObjectDecoratorVolatile
    {
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly IPropagateContentHandler _propagateContentHandler;
        private readonly ISignatureProvider _signatureProvider;
        private readonly SubstituteParser _substituteParser;
        private readonly ConcurrentDictionary<long, long> _callIds = new ();

        public SubstituteReplayDecorator(
            IBinarySerializeProvider binarySerializeProvider,
            IPropagateContentHandler propagateContentHandler,
            IStreamBatchProvider streamBatchProvider,
            ISignatureProvider signatureProvider)
        {
            _binarySerializeProvider = binarySerializeProvider;
            _propagateContentHandler = propagateContentHandler;
            _signatureProvider = signatureProvider;
            var substituteData = new MemoryStream(_propagateContentHandler.ParentData["SubstituteData"]);
            _substituteParser = new SubstituteParser(binarySerializeProvider, streamBatchProvider);
            _substituteParser.Load(substituteData);
        }

        public string Id => "Lide.Substitute.Replay";

        public void ExecuteBeforeInvoke(MethodMetadataVolatile methodMetadata)
        {
            var signature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.AllSet);
            var before = _substituteParser.Befores.FirstOrDefault(x => x.MethodSignature == signature);
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
            var after = _substituteParser.Afters.FirstOrDefault(x => x.CallId == callId);
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
    }
}