using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;

namespace Lide.Core.Provider;

public class SignatureProvider : ISignatureProvider
{
    // Frames skipped:
    // 1. ExtractCallerInformation
    // 2. Proxy invocation
    private const int InitialFramesToSkip = 2;
    private static readonly List<string> ExcludeNamespaces = new () { "System", "Microsoft", "Lide" };

    public string GetCallerSignature()
    {
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

            return $"{frameFileName}-{frameMethodInfo.Name}:{frameLineNumber}";
        }

        return string.Empty;
    }

    public string GetMethodSignature(MethodInfo methodInfo, SignatureOptions signatureOptions)
    {
        var declaringTypeName = ExtractFullTypeName(methodInfo.DeclaringType, signatureOptions, new SignatureRequest { IsBase = true });
        var methodName = methodInfo.Name;
        var methodGenerics = string.Empty;
        var returnType = ExtractFullTypeName(methodInfo.ReturnType, signatureOptions, new SignatureRequest { IsReturn = true });

        if (methodInfo.IsGenericMethod)
        {
            var genericArguments = methodInfo.GetGenericArguments()
                .Select(x => ExtractFullTypeName(x, signatureOptions, new SignatureRequest { IsGeneric = true }))
                .ToArray();

            methodGenerics = $"<{string.Join(",", genericArguments)}>";
        }

        var methodParams = string.Join(",", methodInfo.GetParameters()
            .Select(p => ExtractFullTypeName(p.ParameterType, signatureOptions, new SignatureRequest { IsParameter = true }))
            .ToArray());

        return $"{declaringTypeName}.{methodName}{methodGenerics}({methodParams}):{returnType}";
    }

    public string ExtractFullTypeName(Type type, SignatureOptions signatureOptions, SignatureRequest signatureRequest)
    {
        var assemblyName = $"[{type.Assembly.GetName().Name}]";
        var @namespace = $"{type.Namespace}+";
        var name = type.Name;
        var generics = string.Empty;

        if (type.IsGenericType)
        {
            var genericArguments = type.GetGenericArguments()
                .Select(x => ExtractFullTypeName(x, signatureOptions, new SignatureRequest { IsGeneric = true }))
                .ToArray();

            generics = $"<{string.Join(",", genericArguments)}>";
        }

        var includeAssembly = (signatureRequest.IsBase && signatureOptions.IncludeAssemblyForBase)
                              || (signatureRequest.IsParameter && signatureOptions.IncludeAssemblyForParams)
                              || (signatureRequest.IsReturn && signatureOptions.IncludeAssemblyForReturn)
                              || (signatureRequest.IsGeneric && signatureOptions.IncludeAssemblyForGeneric);

        var includeNamespace = (signatureRequest.IsBase && signatureOptions.IncludeNamespaceForBase)
                               || (signatureRequest.IsParameter && signatureOptions.IncludeNamespaceForParams)
                               || (signatureRequest.IsReturn && signatureOptions.IncludeNamespaceForReturn)
                               || (signatureRequest.IsGeneric && signatureOptions.IncludeNamespaceForGeneric);

        assemblyName = includeAssembly ? assemblyName : string.Empty;
        @namespace = includeNamespace ? @namespace : string.Empty;

        return $"{assemblyName}{@namespace}{name}{generics}";
    }
}