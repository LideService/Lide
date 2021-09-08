using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lide.AsyncProxy.Tests.Stubs
{
    public class DefferedFunctionProxy : DispatchProxyAsync, ICallOnInvoke
    {
        private object _callOnInvokeAsyncT;
        public Func<MethodInfo, object[], object> CallOnInvoke { get; set; }

        public Func<MethodInfo, object[], Task> CallOnInvokeAsync { get; set; }

        public void SetCallOnInvokeAsyncT<T>(Func<MethodInfo, object[], Task<T>> callOnInvokeAsyncT)
        {
            _callOnInvokeAsyncT = callOnInvokeAsyncT;
        }


        public override object Invoke(MethodInfo method, object[] args)
        {
            return CallOnInvoke(method, args);
        }

        public override Task InvokeAsync(MethodInfo method, object[] args)
        {
            return CallOnInvokeAsync(method, args);
        }

        public override Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
        {
            return ((Func<MethodInfo, object[], Task<T>>)_callOnInvokeAsyncT)(method, args);
        }
    }
}