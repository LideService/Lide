using System.Reflection;

namespace Lide.TracingProxy.Contract
{
    public delegate object MethodInfoCompiled(object instance, object[] arguments);

    public interface IMethodInfoProvider
    {
        MethodInfoCompiled GetMethodInfoCompiled(MethodInfo methodInfo);
    }
}