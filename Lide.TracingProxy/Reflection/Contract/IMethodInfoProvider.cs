using System.Reflection;

namespace Lide.TracingProxy.Reflection.Contract
{
    public delegate object MethodInfoCompiled(object instance, object[] arguments);

    public interface IMethodInfoProvider
    {
        MethodInfoCompiled GetMethodInfoCompiled(MethodInfo methodInfo);
    }
}