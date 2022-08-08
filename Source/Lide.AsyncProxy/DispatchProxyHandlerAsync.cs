using System.Diagnostics;
using System.Threading.Tasks;
using Lide.AsyncProxy.DispatchProxyGeneratorAsync;

namespace Lide.AsyncProxy
{
    public sealed class DispatchProxyHandlerAsync
    {
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object InvokeHandle(object[] args)
        {
            return ProxyGeneratorAsync.Invoke(args);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        public Task InvokeAsyncHandle(object[] args)
        {
            return ProxyGeneratorAsync.InvokeAsync(args);
        }

        [DebuggerStepThrough]
        [DebuggerHidden]
        public Task<T> InvokeAsyncHandleT<T>(object[] args)
        {
            return ProxyGeneratorAsync.InvokeAsync<T>(args);
        }
    }
}