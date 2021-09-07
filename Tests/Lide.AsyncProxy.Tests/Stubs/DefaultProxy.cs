using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lide.AsyncProxy.Tests.Stubs
{
    public class DefaultProxy : DispatchProxyAsync
    {
        public Func<MethodInfo, object[], object> CallOnInvoke { get; set; }

        public Func<MethodInfo, object[], Task> CallOnInvokeAsync { get; set; }

        public Func<MethodInfo, object[], Task<object>> CallOnInvokeAsyncT { get; set; }

        public override object Invoke(MethodInfo method, object[] args)
        {
            return CallOnInvoke(method, args);
        }

        public override async Task InvokeAsync(MethodInfo method, object[] args)
        {
            await CallOnInvokeAsync(method, args);
        }

        public override async Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
        {
            var result = await CallOnInvokeAsyncT(method, args);
            return (T)result;
        }
    }
    
    public class DefaultProxyForException : DispatchProxyAsync
    {
        public object OriginalObject { get; set; }

        public override object Invoke(MethodInfo method, object[] args)
        {
            try
            {
                return method.Invoke(OriginalObject, args);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
                throw;
            }
        }

        public override Task InvokeAsync(MethodInfo method, object[] args)
        {
            return (Task)method.Invoke(OriginalObject, args);
        }

        public override Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
        {
            return (Task<T>)method.Invoke(OriginalObject, args);
        }
    }
}