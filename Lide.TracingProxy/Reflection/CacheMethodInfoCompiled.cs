using System;
using System.Collections.Concurrent;
using System.Reflection;
using Lide.TracingProxy.Contract;

namespace Lide.TracingProxy.Reflection
{
    public class CacheMethodInfoCompiled : IMethodInfoCache
    {
        public static readonly IMethodInfoCache Singleton = new CacheMethodInfoCompiled();

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<MethodInfo, MethodInfoCompiled>> _cache = new ();

        public bool TryAdd(Type originalObjectType, MethodInfo methodInfo, MethodInfoCompiled methodInfoCompiled)
        {
            if (!_cache.ContainsKey(originalObjectType))
            {
                _cache[originalObjectType] = new ConcurrentDictionary<MethodInfo, MethodInfoCompiled>();
            }

            if (!_cache[originalObjectType].ContainsKey(methodInfo))
            {
                _cache[originalObjectType][methodInfo] = methodInfoCompiled;
                return true;
            }

            return false;
        }

        public bool Exists(Type originalObjectType, MethodInfo methodInfo)
        {
            return _cache.ContainsKey(originalObjectType) && _cache[originalObjectType].ContainsKey(methodInfo);
        }

        public MethodInfoCompiled GetValue(Type originalObjectType, MethodInfo methodInfo)
        {
            if (_cache.ContainsKey(originalObjectType) && _cache[originalObjectType].ContainsKey(methodInfo))
            {
                return _cache[originalObjectType][methodInfo];
            }

            throw new ArgumentOutOfRangeException();
        }

        public MethodInfoCompiled GetOrAdd(Type originalObjectType, MethodInfo methodInfo, Func<MethodInfoCompiled> delegateCreator)
        {
            if (!_cache.ContainsKey(originalObjectType))
            {
                _cache[originalObjectType] = new ConcurrentDictionary<MethodInfo, MethodInfoCompiled>();
            }

            if (!_cache[originalObjectType].ContainsKey(methodInfo))
            {
                _cache[originalObjectType][methodInfo] = delegateCreator.Invoke();
            }

            return _cache[originalObjectType][methodInfo];
        }
    }
}