using System;
using System.Reflection;

namespace Lide.TracingProxy.Contract
{
    public interface IMethodInfoCache
    {
        bool TryAdd(Type originalObjectType, MethodInfo methodInfo, MethodInfoCompiled methodInfoCompiled);
        bool Exists(Type originalObjectType, MethodInfo methodInfo);
        MethodInfoCompiled GetValue(Type originalObjectType, MethodInfo methodInfo);
        MethodInfoCompiled GetOrAdd(Type originalObjectType, MethodInfo methodInfo, Func<MethodInfoCompiled> delegateCreator);
    }
}