using System;
using System.Linq;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Reflection;

namespace Lide.TracingProxy.DecoratedProxy
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
            if (decorator != null)
            {
                _decorators.Add(decorator);
            }

            return this;
        }

        public IProxyCompositorTyped<TInterface> SetDecorators(params IObjectDecorator[] decorators)
        {
            _decorators.AddRange(decorators.Where(x => x != null));
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

        public IProxyCompositorTyped<TInterface> SetLogErrorAction(Action<string> logError)
        {
            _logError = logError;
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