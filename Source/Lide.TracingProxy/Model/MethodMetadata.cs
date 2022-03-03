using System.Reflection;

namespace Lide.TracingProxy.Model
{
    public class MethodMetadata
    {
        public MethodMetadata(MethodMetadataVolatile methodMetadataVolatile, ParametersMetadata parametersMetadata, ReturnMetadata returnMetadata)
        {
            PlainObject = methodMetadataVolatile.PlainObject;
            IsSingleton = methodMetadataVolatile.IsSingleton;
            MethodInfo = methodMetadataVolatile.MethodInfo;
            ParametersMetadata = parametersMetadata;
            ReturnMetadata = returnMetadata;
            CallId = methodMetadataVolatile.CallId;
        }

        public long CallId { get; }
        public object PlainObject { get; }
        public bool IsSingleton { get; }
        public MethodInfo MethodInfo { get; }
        public ParametersMetadata ParametersMetadata { get; }
        public ReturnMetadata ReturnMetadata { get; }
    }
}