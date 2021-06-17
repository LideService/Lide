namespace Lide.TracingProxy.Model
{
    public class MethodData
    {
        public object[] MethodInputArguments { get; set; } = null!;
        public object MethodResult { get; set; } = null!;
    }
}