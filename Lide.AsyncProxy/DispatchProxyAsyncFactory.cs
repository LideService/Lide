using static Lide.AsyncProxy.DispatchProxyGeneratorAsync.DispatchProxyGeneratorAsync;

namespace Lide.AsyncProxy
{
    public static class DispatchProxyAsyncFactory
    {
        public static T Create<T, TProxy>()
            where TProxy : DispatchProxyAsync
        {
            return (T)CreateProxyInstance(typeof(TProxy), typeof(T));
        }
    }
}