using Lide.TracingProxy.Contract;
using Lide.TracingProxy.DataProcessors.Contract;

namespace Lide.TracingProxy.Reflection.Contract
{
    public interface IProxyCompositorGeneric<TOriginalObject>
        where TOriginalObject : class
    {
        IProxyCompositorGeneric<TOriginalObject> SetOriginalObject(TOriginalObject originalObject);
        IProxyCompositorGeneric<TOriginalObject> SetDecorator(IObjectDecorator decorator);
        IProxyCompositorGeneric<TOriginalObject> SetDecorators(params IObjectDecorator[] decorators);
        IProxyCompositorGeneric<TOriginalObject> SetFastMethodInfoCache(IFastMethodInfoCache fastMethodInfoCache);
        IProxyCompositorGeneric<TOriginalObject> SetScopeTracker(IScopeTracker scopeTracker);
        TOriginalObject GetDecoratedObject();
        TOriginalObject? GetDecoratedObjectSafe();
    }
}