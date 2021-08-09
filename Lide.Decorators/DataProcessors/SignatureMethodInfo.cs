using System;
using System.Linq;
using System.Reflection;
using Lide.Decorators.Contract;

namespace Lide.Decorators.DataProcessors
{
    public class SignatureMethodInfo : ISignatureMethodInfo
    {
        public string GetMethodSignature(MethodInfo methodInfo)
        {
            var declaringTypeName = ExtractFullTypeName(methodInfo.DeclaringType);
            var methodName = methodInfo.Name;
            var methodGenerics = string.Empty;
            var returnType = ExtractFullTypeName(methodInfo.ReturnType);

            if (methodInfo.IsGenericMethod)
            {
                var genericArguments = methodInfo.GetGenericArguments()
                    .Select(ExtractFullTypeName)
                    .ToArray();

                methodGenerics = $"<{string.Join(",", genericArguments)}>";
            }

            var methodParams = string.Join(",", methodInfo.GetParameters()
                .Select(p => ExtractFullTypeName(p.ParameterType))
                .ToArray());

            return $"{declaringTypeName}.{methodName}{methodGenerics}({methodParams}):{returnType}";
        }

        public string ExtractFullTypeName(Type type)
        {
            var assemblyName = type.Assembly.GetName().Name;
            var @namespace = type.Namespace;
            var name = type.Name;
            var generics = string.Empty;

            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments()
                    .Select(ExtractFullTypeName)
                    .ToArray();

                generics = $"<{string.Join(",", genericArguments)}>";
            }

            return $"{assemblyName}:{@namespace}.{name}{generics}";
        }
    }
}