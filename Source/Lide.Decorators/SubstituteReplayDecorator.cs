using System;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.Decorators.Substitute;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class SubstituteReplayDecorator : IObjectDecoratorVolatile
    {
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly ISignatureProvider _signatureProvider;
        private readonly ISubstituteLoader _substituteLoader;

        public SubstituteReplayDecorator(
            IBinarySerializeProvider binarySerializeProvider,
            ISignatureProvider signatureProvider,
            ISubstituteLoader substituteLoader)
        {
            _binarySerializeProvider = binarySerializeProvider;
            _signatureProvider = signatureProvider;
            _substituteLoader = substituteLoader;
        }

        public string Id => "Lide.Substitute.Replay";

        public void ExecuteBeforeInvoke(MethodMetadataVolatile methodMetadata)
        {
            var methodSignature = _signatureProvider.GetMethodSignature(methodMetadata.MethodInfo, SignatureOptions.AllSet);
            var before = _substituteLoader.GetBefore(methodMetadata.CallId, methodSignature);
            var parameters = (object[])_binarySerializeProvider.Deserialize(before.InputParameters);
            methodMetadata.ParametersMetadataVolatile.SetParameters(parameters);
        }

        public void ExecuteAfterResult(MethodMetadataVolatile methodMetadata)
        {
            var after = _substituteLoader.GetAfter(methodMetadata.CallId);
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