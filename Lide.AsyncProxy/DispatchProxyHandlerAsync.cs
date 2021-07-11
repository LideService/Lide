using System.Threading.Tasks;
using Lide.AsyncProxy.DispatchProxyGeneratorAsync;

namespace Lide.AsyncProxy
{
    public class DispatchProxyHandlerAsync
    {
        public object InvokeHandle(object[] args)
        {
            return ProxyGeneratorAsync.Invoke(args);
        }

        public Task InvokeAsyncHandle(object[] args)
        {
            return ProxyGeneratorAsync.InvokeAsync(args);
        }

        public Task<T> InvokeAsyncHandleT<T>(object[] args)
        {
            return ProxyGeneratorAsync.InvokeAsync<T>(args);
        }
    }
}