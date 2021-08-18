using Lide.TracingProxy.Contract;

namespace Lide.Decorators
{
    public class SubstituteRecordDecorator : IObjectDecorator
    {
        public string Id { get; } = "Lide.Substitute.Record";
        public bool IsVolatile { get; } = false;
    }
}