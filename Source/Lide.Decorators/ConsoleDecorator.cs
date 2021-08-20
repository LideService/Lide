using Lide.Core.Contract.Facade;
using Lide.TracingProxy.Contract;

namespace Lide.Decorators
{
    public class ConsoleDecorator : IObjectDecorator
    {
        private readonly IConsoleFacade _consoleFacade;

        public ConsoleDecorator(IConsoleFacade consoleFacade)
        {
            _consoleFacade = consoleFacade;
        }

        public string Id { get; } = "Lide.Console";
        public bool IsVolatile { get; } = false;
    }
}