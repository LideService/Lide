using System.Reflection;
using System.Threading.Tasks;

namespace Lide.AsyncProxy;

public abstract class DispatchProxyAsync
{
    public abstract object Invoke(MethodInfo method, object[] args);

    public abstract Task InvokeAsync(MethodInfo method, object[] args);

    public abstract Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args);
}