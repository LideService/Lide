using System;
using System.Reflection;

namespace Lide.Core.Contract.Provider
{
    public interface ISignatureProvider
    {
        string GetCallerSignature();
        string GetMethodSignature(MethodInfo methodInfo);
        string ExtractFullTypeName(Type type);
    }
}