using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Lide.AsyncProxy.Tests.Proxy
{
    public interface ICallOnInvoke
    {
        Func<MethodInfo, object[], object> CallOnInvoke { get; set; }
    }
    
    public class BaseDispatchProxy : DispatchProxyAsync, ICallOnInvoke
    {
        public Func<MethodInfo, object[], object> CallOnInvoke { get; set; }
        
        public override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return CallOnInvoke(targetMethod, args);
        }

        public override Task InvokeAsync(MethodInfo targetMethod, object[] args)
        {
            return Task.FromResult(CallOnInvoke(targetMethod, args));
        }

        public override Task<T> InvokeAsyncT<T>(MethodInfo targetMethod, object[] args)
        {
            return Task.FromResult((T)CallOnInvoke(targetMethod, args));
        }
    }
}