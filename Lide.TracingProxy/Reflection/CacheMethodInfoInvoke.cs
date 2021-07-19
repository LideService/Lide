using System;
using System.Reflection;
using Lide.TracingProxy.Reflection.Contract;

namespace Lide.TracingProxy.Reflection
{
    public class CacheMethodInfoInvoke : IMethodInfoCache
    {
        public static IMethodInfoCache Singleton = new CacheMethodInfoInvoke();

        public bool TryAdd(Type originalObjectType, MethodInfo methodInfo, MethodInfoCompiled methodInfoCompiled)
        {
            return true;
        }

        public bool Exists(Type originalObjectType, MethodInfo methodInfo)
        {
            return true;
        }

        public MethodInfoCompiled GetValue(Type originalObjectType, MethodInfo methodInfo)
        {
            return methodInfo.Invoke;
        }

        public MethodInfoCompiled GetOrAdd(Type originalObjectType, MethodInfo methodInfo, Func<MethodInfoCompiled> delegateCreator)
        {
            return methodInfo.Invoke;
        }
    }
}