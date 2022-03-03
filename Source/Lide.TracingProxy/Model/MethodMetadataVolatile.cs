using System.Reflection;

namespace Lide.TracingProxy.Model
{
    public class MethodMetadataVolatile
    {
        public MethodMetadataVolatile(
            object plainObject,
            bool isSingleton,
            MethodInfo methodInfo,
            ParametersMetadataVolatile parametersMetadataVolatile,
            ReturnMetadataVolatile returnMetadataVolatile,
            long callId)
        {
            PlainObject = plainObject;
            IsSingleton = isSingleton;
            MethodInfo = methodInfo;
            ParametersMetadataVolatile = parametersMetadataVolatile;
            ReturnMetadataVolatile = returnMetadataVolatile;
            CallId = callId;
        }

        public MethodMetadataVolatile(
            MethodMetadataVolatile methodMetadataVolatile,
            ReturnMetadataVolatile returnMetadataVolatile)
        {
            IsSingleton = methodMetadataVolatile.IsSingleton;
            PlainObject = methodMetadataVolatile.PlainObject;
            MethodInfo = methodMetadataVolatile.MethodInfo;
            ParametersMetadataVolatile = methodMetadataVolatile.ParametersMetadataVolatile;
            ReturnMetadataVolatile = returnMetadataVolatile;
            CallId = methodMetadataVolatile.CallId;
        }

        public long CallId { get; }
        public object PlainObject { get; }
        public bool IsSingleton { get; }
        public MethodInfo MethodInfo { get; }
        public ParametersMetadataVolatile ParametersMetadataVolatile { get; }
        public ReturnMetadataVolatile ReturnMetadataVolatile { get; }
    }
}