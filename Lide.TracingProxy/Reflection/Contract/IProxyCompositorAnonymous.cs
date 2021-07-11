using Lide.TracingProxy.Contract;
using Lide.TracingProxy.DataProcessors.Contract;

namespace Lide.TracingProxy.Reflection.Contract
{
    public interface IProxyCompositorAnonymous
    {
        IProxyCompositorAnonymous SetOriginalObject(object originalObject);
        IProxyCompositorAnonymous SetDecorator(IObjectDecorator decorator);
        IProxyCompositorAnonymous SetDecorators(params IObjectDecorator[] decorators);
        IProxyCompositorAnonymous SetFastMethodInfoCache(IFastMethodInfoCache fastMethodInfoCache);
        IProxyCompositorAnonymous SetScopeTracker(IScopeTracker scopeTracker);
        object GetDecoratedObject();
        object? GetDecoratedObjectSafe();
    }
}