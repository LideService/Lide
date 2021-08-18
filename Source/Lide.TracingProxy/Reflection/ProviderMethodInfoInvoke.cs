using System.Reflection;
using Lide.TracingProxy.Contract;

namespace Lide.TracingProxy.Reflection
{
    public class ProviderMethodInfoInvoke : IMethodInfoProvider
    {
        public static readonly IMethodInfoProvider Singleton = new ProviderMethodInfoInvoke();

        public MethodInfoCompiled GetMethodInfoCompiled(MethodInfo methodInfo)
        {
            return methodInfo.Invoke;
        }
    }
}