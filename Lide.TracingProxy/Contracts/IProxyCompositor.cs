namespace Lide.TracingProxy.Contracts
{
    public interface IProxyCompositor<TOriginalObject>
        where TOriginalObject : class
    {
        IProxyCompositor<TOriginalObject> SetOriginalObject(TOriginalObject originalObject);
        IProxyCompositor<TOriginalObject> SetDecorator(IProxyDecorator decorator);
        IProxyCompositor<TOriginalObject> SetDecorator(params IProxyDecorator[] decorators);
        IProxyCompositor<TOriginalObject> SetFastMethodInfoCache(IFastMethodInfoCache fastMethodInfoCache);
        IProxyCompositor<TOriginalObject> SetScopeTracker(IScopeTracker scopeTracker);
        TOriginalObject GetDecoratedObject();
        TOriginalObject? GetDecoratedObjectSafe();
    }
}