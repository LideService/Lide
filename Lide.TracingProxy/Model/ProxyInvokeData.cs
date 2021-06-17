namespace Lide.TracingProxy.Model
{
    public class ProxyInvokeData
    {
        public string ScopeGuid { get; set; } = null!;
        public CallerInformation? CallerInformation { get; set; }
        public MethodInformation? MethodInformation { get; set; }
        public MethodData? MethodData { get; set; }
    }
}