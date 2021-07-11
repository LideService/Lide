using System.Reflection;
using Lide.TracingProxy.Reflection.Contract;

namespace Lide.TracingProxy.Reflection
{
    public class ProviderMethodInfoInvoke : IMethodInfoProvider
    {
        public static IMethodInfoProvider Singleton = new ProviderMethodInfoInvoke();

        public MethodInfoCompiled GetMethodInfoCompiled(MethodInfo methodInfo)
        {
            return methodInfo.Invoke;
        }
    }
}