using System;
using System.Reflection;

namespace Lide.Decorators.Contract
{
    public interface ISignatureMethodInfo
    {
        string GetMethodSignature(MethodInfo methodInfo);
        string ExtractFullTypeName(Type type);
    }
}