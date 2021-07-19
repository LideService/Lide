using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Reflection.Contract;

namespace Lide.TracingProxy.Reflection
{
    public partial class ProxyDecoratorTyped<TInterface> : IProxyCompositorTyped<TInterface>
        where TInterface : class
    {
        public IProxyCompositorTyped<TInterface> SetOriginalObject(TInterface originalObject)
        {
            _originalObject = originalObject;
            _originalObjectType = typeof(TInterface);
            return this;
        }

        public IProxyCompositorTyped<TInterface> SetDecorator(IObjectDecorator decorator)
        {
            _decorators.Add(decorator);
            return this;
        }

        public IProxyCompositorTyped<TInterface> SetDecorators(params IObjectDecorator[] decorators)
        {
            _decorators.AddRange(decorators);
            return this;
        }

        public IProxyCompositorTyped<TInterface> SetDelegateMethodInfoCache(IMethodInfoCache methodInfoCache)
        {
            _methodInfoCache = methodInfoCache;
            return this;
        }

        public IProxyCompositorTyped<TInterface> SetDelegateMethodInfoProvider(IMethodInfoProvider methodInfoProvider)
        {
            _methodInfoProvider = methodInfoProvider;
            return this;
        }

        public TInterface GetDecoratedObject()
        {
            _methodInfoCache ??= CacheMethodInfoInvoke.Singleton;
            _methodInfoProvider ??= ProviderMethodInfoInvoke.Singleton;

            if (_originalObject == null || _decorators.Count == 0)
            {
                return _originalObject;
            }

            return (TInterface)(object)this;
        }
    }
}