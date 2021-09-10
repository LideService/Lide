using System;
using System.Reflection;

namespace Lide.AsyncProxy.Tests.Proxy
{
    public interface ICallOnInvoke
    {
        Func<MethodInfo, object[], object> CallOnInvoke { get; set; }
    }
    
    public class BaseDispatchProxy : DispatchProxy, ICallOnInvoke
    {
        public Func<MethodInfo, object[], object> CallOnInvoke { get; set; }
        
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return CallOnInvoke(targetMethod, args);
        }
    }
}