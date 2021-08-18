namespace Lide.TracingProxy.Contract
{
    public interface IProxyCompositorAnonymous
    {
        IProxyCompositorAnonymous SetOriginalObject(object originalObject);
        IProxyCompositorAnonymous SetDecorator(IObjectDecorator decorator);
        IProxyCompositorAnonymous SetDecorators(params IObjectDecorator[] decorators);
        IProxyCompositorAnonymous SetDelegateMethodInfoCache(IMethodInfoCache fastMethodInfoCache);
        IProxyCompositorAnonymous SetDelegateMethodInfoProvider(IMethodInfoProvider fastMethodInfoCache);
        object GetDecoratedObject();
    }
}