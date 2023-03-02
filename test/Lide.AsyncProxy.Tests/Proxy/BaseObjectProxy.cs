using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lide.AsyncProxy.Tests.Proxy
{
    public class BaseObjectProxy : DispatchProxyAsync
    {
        public object BaseObject { get; set; }

        public override object Invoke(MethodInfo method, object[] args)
        {
            try
            {
                return method.Invoke(BaseObject, args);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
                throw;
            }
        }

        public override Task InvokeAsync(MethodInfo method, object[] args)
        {
            try
            {
                return (Task)method.Invoke(BaseObject, args);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
                throw;
            }
        }

        public override Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
        {
            try
            {
                return (Task<T>)method.Invoke(BaseObject, args);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
                throw;
            }
        }
    }
}