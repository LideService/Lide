using System;
using System.Reflection;
using Lide.Core.Contract.Wrapper;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.Decorators
{
    public class ConsoleDecorator : IObjectDecorator
    {
        private readonly IConsoleWrapper _consoleWrapper;
        public string Id { get; } = "Lide.Console";
        public bool IsVolatile { get; } = false;

        public ConsoleDecorator(IConsoleWrapper consoleWrapper)
        {
            _consoleWrapper = consoleWrapper;
        }
    }
}