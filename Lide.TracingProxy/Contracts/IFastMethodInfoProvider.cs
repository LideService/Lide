using System.Reflection;

namespace Lide.TracingProxy.Contracts
{
    public delegate object MethodInfoDelegate(object instance, object[] arguments);

    public interface IFastMethodInfoProvider
    {
        MethodInfoDelegate CompileMethodInfo(MethodInfo methodInfo);
    }
}