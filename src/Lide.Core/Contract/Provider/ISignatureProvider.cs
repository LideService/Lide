using System;
using System.Reflection;
using Lide.Core.Model;

namespace Lide.Core.Contract.Provider;

public interface ISignatureProvider
{
    string GetCallerSignature();
    string GetMethodSignature(MethodInfo methodInfo, SignatureOptions signatureOptions);
    string ExtractFullTypeName(Type type, SignatureOptions signatureOptions, SignatureRequest signatureRequest);
}