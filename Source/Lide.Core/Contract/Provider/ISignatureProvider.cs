using System;
using System.Reflection;

namespace Lide.Core.Contract.Provider
{
    public interface ISignatureProvider
    {
        string GetCallerSignature();
        string GetMethodSignature(MethodInfo methodInfo, bool includeAssembly = false);
        string ExtractFullTypeName(Type type, bool includeAssembly = false);
    }
}