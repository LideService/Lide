using System;
using System.Collections.Concurrent;
using System.Reflection;
using Lide.TracingProxy.Contracts;

namespace Lide.TracingProxy.Reflection
{
    public class FastMethodInfoCache : IFastMethodInfoCache
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<int, MethodInfoDelegate>> _cache;
        private readonly IFastMethodInfoProvider _fastMethodInfoProvider;

        public FastMethodInfoCache(IFastMethodInfoProvider fastMethodInfoProvider)
        {
            _cache = new ConcurrentDictionary<Type, ConcurrentDictionary<int, MethodInfoDelegate>>();
            _fastMethodInfoProvider = fastMethodInfoProvider;
        }

        public MethodInfoDelegate GetCompiledMethodInfo(Type originalObjectType, MethodInfo methodInfo)
        {
            if (!_cache.ContainsKey(originalObjectType))
            {
                _cache.TryAdd(originalObjectType, new ConcurrentDictionary<int, MethodInfoDelegate>());
            }

            int methodHash = methodInfo.GetHashCode();
            if (!_cache[originalObjectType].ContainsKey(methodHash))
            {
                MethodInfoDelegate compiledMethodInfo = _fastMethodInfoProvider.CompileMethodInfo(methodInfo);
                _cache[originalObjectType].TryAdd(methodHash, compiledMethodInfo);
            }

            return _cache[originalObjectType][methodHash];
        }
    }
}