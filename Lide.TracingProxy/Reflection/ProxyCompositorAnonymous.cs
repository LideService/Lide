using System;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.DataProcessors.Contract;
using Lide.TracingProxy.Reflection.Contract;

namespace Lide.TracingProxy.Reflection
{
    public partial class ProxyDecoratorAnonymous :  IProxyCompositorAnonymous
    {
        public static IProxyCompositorAnonymous CreateProxyDecorator(Type originalObjectInterfaceType)
        {
            return null!; //;(ProxyDecoratorAnonymous)(object)DispatchProxyAsyncFactory.Create(originalObjectInterfaceType, typeof(ProxyDecoratorAnonymous));
        }

        public IProxyCompositorAnonymous SetOriginalObject(object originalObject)
        {
            _originalObject = originalObject;
            _originalObjectType = originalObject.GetType();
            return this;
        }

        public IProxyCompositorAnonymous SetDecorator(IObjectDecorator decorator)
        {
            _decorators.Add(decorator);
            return this;
        }

        public IProxyCompositorAnonymous SetDecorators(params IObjectDecorator[] decorators)
        {
            _decorators.AddRange(decorators);
            return this;
        }

        public IProxyCompositorAnonymous SetFastMethodInfoCache(IFastMethodInfoCache fastMethodInfoCache)
        {
            _fastMethodInfoCache = fastMethodInfoCache;
            return this;
        }

        public IProxyCompositorAnonymous SetScopeTracker(IScopeTracker scopeTracker)
        {
            _scopeTracker = scopeTracker;
            return this;
        }
        
        public object GetDecoratedObject()
        {
            return GetDecoratedObjectInternal(true)!;
        }
        
        public object? GetDecoratedObjectSafe()
        {
            return GetDecoratedObjectInternal(false);
        }

        private object? GetDecoratedObjectInternal(bool throwOnError)
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

            return this;
        }
    }
}