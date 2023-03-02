using System;
using Lide.AsyncProxy;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.DecoratedProxy;

namespace Lide.TracingProxy;

public static class ProxyDecoratorFactory
{
    public static IProxyCompositor<TInterface> CreateProxyDecorator<TInterface>()
        where TInterface : class
    {
        return (ProxyDecorator<TInterface>)(object)DispatchProxyAsyncFactory.Create<TInterface, ProxyDecorator<TInterface>>();
    }

    public static IProxyCompositor<object> CreateProxyDecorator(Type tInterface)
    {
        return (ProxyDecorator<object>)DispatchProxyAsyncFactory.Create(tInterface, typeof(ProxyDecorator<object>));
    }
}