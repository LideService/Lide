using System;
using System.Reflection;
using Lide.Core.Contract.Facade;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class ConsoleDecorator : IObjectDecorator
    {
        private readonly IConsoleFacade _consoleFacade;
        public string Id { get; } = "Lide.Console";
        public bool IsVolatile { get; } = false;

        public ConsoleDecorator(IConsoleFacade consoleFacade)
        {
            _consoleFacade = consoleFacade;
        }
    }
}