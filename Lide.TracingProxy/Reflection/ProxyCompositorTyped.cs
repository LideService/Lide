using System;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.DataProcessors.Contract;
using Lide.TracingProxy.Reflection.Contract;

namespace Lide.TracingProxy.Reflection
{
    public partial class ProxyDecoratorTyped<TOriginalObject> :  IProxyCompositorGeneric<TOriginalObject>
        where TOriginalObject : class
    {
        public static IProxyCompositorGeneric<TOriginalObject> CreateProxyDecorator()
        {
            return null!;//(ProxyDecoratorTyped<TOriginalObject>)(object)DispatchProxyAsyncFactory. Create<TOriginalObject, ProxyDecoratorTyped<TOriginalObject>>();
        }

        public IProxyCompositorGeneric<TOriginalObject> SetOriginalObject(TOriginalObject originalObject)
        {
            _originalObject = originalObject;
            _originalObjectType = typeof(TOriginalObject);
            return this;
        }

        public IProxyCompositorGeneric<TOriginalObject> SetDecorator(IObjectDecorator decorator)
        {
            _decorators.Add(decorator);
            return this;
        }

        public IProxyCompositorGeneric<TOriginalObject> SetDecorators(params IObjectDecorator[] decorators)
        {
            _decorators.AddRange(decorators);
            return this;
        }

        public IProxyCompositorGeneric<TOriginalObject> SetFastMethodInfoCache(IFastMethodInfoCache fastMethodInfoCache)
        {
            _fastMethodInfoCache = fastMethodInfoCache;
            return this;
        }

        public IProxyCompositorGeneric<TOriginalObject> SetScopeTracker(IScopeTracker scopeTracker)
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