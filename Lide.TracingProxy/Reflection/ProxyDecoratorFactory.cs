using System;
using Lide.AsyncProxy;
using Lide.TracingProxy.Reflection.Contract;

namespace Lide.TracingProxy.Reflection
{
    public static class ProxyDecoratorFactory
    {
        public static IProxyCompositorTyped<TInterface> CreateProxyDecorator<TInterface>()
            where TInterface : class
        {
            return (ProxyDecoratorTyped<TInterface>)(object)DispatchProxyAsyncFactory.Create<TInterface, ProxyDecoratorTyped<TInterface>>();
        }

        public static IProxyCompositorTyped<object> CreateProxyDecorator(Type tInterface)
        {
            return (ProxyDecoratorTyped<object>)DispatchProxyAsyncFactory.Create(tInterface, typeof(ProxyDecoratorTyped<object>));
        }
    }
}