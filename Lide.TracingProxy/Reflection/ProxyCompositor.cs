using System;
using Lide.AsyncProxy;
using Lide.TracingProxy.Contracts;

namespace Lide.TracingProxy.Reflection
{
    public partial class ProxyDecorator<TOriginalObject> 
        where TOriginalObject : class
    {
        public static IProxyCompositor<TOriginalObject> CreateProxyDecorator()
        {
            return (ProxyDecorator<TOriginalObject>)(object)DispatchProxyAsyncFactory.Create<TOriginalObject, ProxyDecorator<TOriginalObject>>();
        }

        public IProxyCompositor<TOriginalObject> SetOriginalObject(TOriginalObject originalObject)
        {
            _originalObject = originalObject;
            _originalObjectType = typeof(TOriginalObject);
            return this;
        }

        public IProxyCompositor<TOriginalObject> SetDecorator(IProxyDecorator decorator)
        {
            _decorators.Add(decorator);
            return this;
        }

        public IProxyCompositor<TOriginalObject> SetDecorator(params IProxyDecorator[] decorators)
        {
            _decorators.AddRange(decorators);
            return this;
        }

        public IProxyCompositor<TOriginalObject> SetFastMethodInfoCache(IFastMethodInfoCache fastMethodInfoCache)
        {
            _fastMethodInfoCache = fastMethodInfoCache;
            return this;
        }

        public IProxyCompositor<TOriginalObject> SetScopeTracker(IScopeTracker scopeTracker)
        {
            _scopeTracker = scopeTracker;
            return this;
        }
        
        public TOriginalObject GetDecoratedObject()
        {
            return GetDecoratedObjectInternal(true)!;
        }
        
        public TOriginalObject? GetDecoratedObjectSafe()
        {
            return GetDecoratedObjectInternal(false);
        }

        private TOriginalObject? GetDecoratedObjectInternal(bool throwOnError)
        {
            if (_originalObject == null && throwOnError)
            {
                throw new ArgumentException("Expected to have the originalObject set");
            }

            if (_fastMethodInfoCache == null && throwOnError)
            {
                throw new ArgumentException("Can't have cache enabled without the actual cache");
            }

            if (_scopeTracker == null && throwOnError)
            {
                throw new ArgumentException("Can't proxy without any scope detection");
            }

            if (_decorators.Count == 0)
            {
                return _originalObject;
            }

            return (TOriginalObject)(object)this;
        }
    }
}