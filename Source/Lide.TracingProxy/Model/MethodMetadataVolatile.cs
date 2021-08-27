using System;
using System.Reflection;

namespace Lide.TracingProxy.Model
{
    public class MethodMetadataVolatile
    {
        public MethodMetadataVolatile(object plainObject, MethodInfo methodInfo, ParametersMetadataVolatile parametersMetadataVolatile, ReturnMetadataVolatile returnMetadataVolatile)
        {
            PlainObject = plainObject;
            MethodInfo = methodInfo;
            ParametersMetadataVolatile = parametersMetadataVolatile;
            ReturnMetadataVolatile = returnMetadataVolatile;
        }

        public MethodMetadataVolatile(MethodMetadataVolatile methodMetadataVolatile, ParametersMetadataVolatile parametersMetadataVolatile, ReturnMetadataVolatile returnMetadataVolatile)
        {
            PlainObject = methodMetadataVolatile.PlainObject;
            MethodInfo = methodMetadataVolatile.MethodInfo;
            ParametersMetadataVolatile = parametersMetadataVolatile;
            ReturnMetadataVolatile = returnMetadataVolatile;
        }

        public MethodMetadataVolatile(MethodMetadataVolatile methodMetadataVolatile, ReturnMetadataVolatile returnMetadataVolatile)
        {
            PlainObject = methodMetadataVolatile.PlainObject;
            MethodInfo = methodMetadataVolatile.MethodInfo;
            ParametersMetadataVolatile = methodMetadataVolatile.ParametersMetadataVolatile;
            ReturnMetadataVolatile = returnMetadataVolatile;
        }

        public object PlainObject { get; }
        public MethodInfo MethodInfo { get; }
        public ParametersMetadataVolatile ParametersMetadataVolatile { get; }
        public ReturnMetadataVolatile ReturnMetadataVolatile { get; }
    }
}