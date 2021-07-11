using Lide.TracingProxy.Contract;
using Lide.TracingProxy.DataProcessors.Contract;

namespace Lide.TracingProxy.Reflection.Contract
{
    public interface IProxyCompositorAnonymous
    {
        IProxyCompositorAnonymous SetOriginalObject(object originalObject);
        IProxyCompositorAnonymous SetDecorator(IObjectDecorator decorator);
        IProxyCompositorAnonymous SetDecorators(params IObjectDecorator[] decorators);
        IProxyCompositorAnonymous SetDelegateMethodInfoCache(IMethodInfoCache fastMethodInfoCache);
        IProxyCompositorAnonymous SetDelegateMethodInfoProvider(IMethodInfoProvider fastMethodInfoCache);
        IProxyCompositorAnonymous SetScopeTracker(IScopeTracker scopeTracker);
        object GetDecoratedObject();
    }
}