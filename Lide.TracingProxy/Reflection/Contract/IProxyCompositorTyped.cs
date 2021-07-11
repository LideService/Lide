using Lide.TracingProxy.Contract;
using Lide.TracingProxy.DataProcessors.Contract;

namespace Lide.TracingProxy.Reflection.Contract
{
    public interface IProxyCompositorTyped<TInterface>
        where TInterface : class
    {
        IProxyCompositorTyped<TInterface> SetOriginalObject(TInterface originalObject);
        IProxyCompositorTyped<TInterface> SetDecorator(IObjectDecorator decorator);
        IProxyCompositorTyped<TInterface> SetDecorators(params IObjectDecorator[] decorators);
        IProxyCompositorTyped<TInterface> SetDelegateMethodInfoCache(IMethodInfoCache methodInfoCache);
        IProxyCompositorTyped<TInterface> SetDelegateMethodInfoProvider(IMethodInfoProvider methodInfoProvider);
        IProxyCompositorTyped<TInterface> SetScopeTracker(IScopeTracker scopeTracker);
        TInterface GetDecoratedObject();
    }
}