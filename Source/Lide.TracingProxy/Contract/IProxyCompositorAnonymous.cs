namespace Lide.TracingProxy.Contract
{
    public interface IProxyCompositorAnonymous
    {
        IProxyCompositorAnonymous SetOriginalObject(object originalObject);
        IProxyCompositorAnonymous SetDecorator(IObjectDecoratorReadonly decoratorReadonly);
        IProxyCompositorAnonymous SetDecorators(params IObjectDecoratorReadonly[] decorators);
        IProxyCompositorAnonymous SetDelegateMethodInfoCache(IMethodInfoCache fastMethodInfoCache);
        IProxyCompositorAnonymous SetDelegateMethodInfoProvider(IMethodInfoProvider fastMethodInfoCache);
        object GetDecoratedObject();
    }
}