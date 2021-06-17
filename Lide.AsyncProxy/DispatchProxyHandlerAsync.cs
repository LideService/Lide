using System.Threading.Tasks;
using static Lide.AsyncProxy.DispatchProxyGeneratorAsync.DispatchProxyGeneratorAsync;

namespace Lide.AsyncProxy
{
    internal class DispatchProxyHandlerAsync
    {
        public object InvokeHandle(object[] args)
        {
            return Invoke(args);
        }

        public Task InvokeAsyncHandle(object[] args)
        {
            return InvokeAsync(args);
        }

        public Task<T> InvokeAsyncHandleT<T>(object[] args)
        {
            return InvokeAsync<T>(args);
        }
    }
}