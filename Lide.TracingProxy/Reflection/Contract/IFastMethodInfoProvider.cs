using System.Reflection;

namespace Lide.TracingProxy.Reflection.Contract
{
    public delegate object MethodInfoDelegate(object instance, object[] arguments);

    public interface IFastMethodInfoProvider
    {
        MethodInfoDelegate CompileMethodInfo(MethodInfo methodInfo);
    }
}