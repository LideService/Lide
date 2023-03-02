using System.Reflection;
using System.Reflection.Emit;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals;

internal sealed class PropertyAccessorInfo
{
    public PropertyAccessorInfo(MethodInfo interfaceGetMethod, MethodInfo interfaceSetMethod)
    {
        InterfaceGetMethod = interfaceGetMethod;
        InterfaceSetMethod = interfaceSetMethod;
    }

    public MethodInfo InterfaceGetMethod { get; }
    public MethodInfo InterfaceSetMethod { get; }
    public MethodBuilder GetMethodBuilder { get; set; }
    public MethodBuilder SetMethodBuilder { get; set; }
}