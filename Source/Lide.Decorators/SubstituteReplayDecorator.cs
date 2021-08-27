using Lide.TracingProxy.Contract;

namespace Lide.Decorators
{
    public class SubstituteReplayDecorator : IObjectDecoratorReadonly
    {
        public string Id { get; } = "Lide.Substitute.Replay";
        public bool IsVolatile { get; } = true;
    }
}