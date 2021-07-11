using System.Reflection;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync
{
    public class ProxyMethodResolverContext
    {
        public ProxyMethodResolverContext(PackedArgs packed, MethodBase method)
        {
            Packed = packed;
            Method = method;
        }

        public PackedArgs Packed { get; }
        public MethodBase Method { get; }
    }
}