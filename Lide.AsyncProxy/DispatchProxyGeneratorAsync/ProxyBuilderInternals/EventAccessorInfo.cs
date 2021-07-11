using System.Reflection;
using System.Reflection.Emit;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals
{
    internal sealed class EventAccessorInfo
    {
        public EventAccessorInfo(MethodInfo interfaceAddMethod, MethodInfo interfaceRemoveMethod, MethodInfo interfaceRaiseMethod)
        {
            InterfaceAddMethod = interfaceAddMethod;
            InterfaceRemoveMethod = interfaceRemoveMethod;
            InterfaceRaiseMethod = interfaceRaiseMethod;
        }

        public MethodInfo InterfaceAddMethod { get; }
        public MethodInfo InterfaceRemoveMethod { get; }
        public MethodInfo InterfaceRaiseMethod { get; }
        public MethodBuilder AddMethodBuilder { get; set; }
        public MethodBuilder RemoveMethodBuilder { get; set; }
        public MethodBuilder RaiseMethodBuilder { get; set; }
    }
}