using System;
using Lide.AsyncProxy.DispatchProxyGeneratorAsync;

namespace Lide.AsyncProxy
{
    public static class DispatchProxyAsyncFactory
    {
        public static TInterface Create<TInterface, TProxy>()
            where TProxy : DispatchProxyAsync
        {
            return (TInterface)ProxyGeneratorAsync.CreateProxyInstance(typeof(TInterface),typeof(TProxy));
        }

        public static object Create(Type tInterface, Type tProxy)
        {
            return ProxyGeneratorAsync.CreateProxyInstance(tInterface, tProxy);
        }
    }
}