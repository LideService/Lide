using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class SignatureProvider  : ISignatureProvider
    {
        // Frames skipped:
        // 1. ExtractCallerInformation
        // 2. Proxy invocation
        private const int InitialFramesToSkip = 2;
        private static readonly List<string> ExcludeNamespaces = new () { "System", "Microsoft" };

        public string GetCallerSignature()
        {
            var skipFirst = false;
            StackTrace stackTrace = new (true);
            for (var i = InitialFramesToSkip; i < stackTrace.FrameCount; i++)
            {
                var stackFrame = stackTrace.GetFrame(i);
                if (stackFrame == null)
                {
                    continue;
                }

                var frameMethodInfo = stackFrame.GetMethod();
                var frameFileName = stackFrame.GetFileName();
                var frameLineNumber = stackFrame.GetFileLineNumber();
                var stackFrameFullMethodName = frameMethodInfo?.DeclaringType?.FullName;
                if (frameMethodInfo == null
                    || stackFrameFullMethodName == null
                    || ExcludeNamespaces.Any(x => stackFrameFullMethodName.StartsWith(x))
                    || string.IsNullOrEmpty(frameFileName)
                    || frameLineNumber == 0)
                {
                    continue;
                }

                if (!skipFirst)
                {
                    skipFirst = true;
                    continue;
                }

                return $"{frameFileName}-{frameMethodInfo.Name}:{frameLineNumber}";
            }

            return string.Empty;
        }
        
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

            return $"[{assemblyName}]:{@namespace}+{name}{generics}";
        }
    }
}