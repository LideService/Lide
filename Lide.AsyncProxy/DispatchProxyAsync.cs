using System.Reflection;
using System.Threading.Tasks;

namespace Lide.AsyncProxy
{
    public abstract class DispatchProxyAsync
    {
        public abstract object Invoke(MethodInfo methodInfo, object[] methodParameters);

        public abstract Task InvokeAsync(MethodInfo methodInfo, object[] methodParameters);

        public abstract Task<T> InvokeAsyncT<T>(MethodInfo methodInfo, object[] methodParameters);
    }
}