using System;
using System.Reflection;
using System.Threading.Tasks;
using Lide.Decorators.Contract;
using Lide.TracingProxy.Contract;

namespace Lide.Decorators.ObjectDecorator
{
    public class ConsoleDecorator : IObjectDecorator
    {
        private readonly IConsoleWrapper _consoleWrapper;
        public string Id { get; } = "Lide.Console";
        public bool Volatile { get; } = false;

        public ConsoleDecorator(IConsoleWrapper consoleWrapper)
        {
            _consoleWrapper = consoleWrapper;
        }
        
        public object[] ExecuteBefore(object originalObject, MethodInfo methodInfo, object[] methodParams)
        {
            throw new NotImplementedException();
        }

        public void ExecuteAfter(object originalObject, MethodInfo methodInfo, object[] methodParams)
        {
            throw new NotImplementedException();
        }

        public T ExecuteAfter<T>(object originalObject, MethodInfo methodInfo, object[] methodParams, T methodResult)
        {
            throw new NotImplementedException();
        }

        public Exception ExecuteException(object originalObject, MethodInfo methodInfo, object[] methodParams, Exception exception)
        {
            throw new NotImplementedException();
        }

        public AggregateException ExecuteException(object originalObject, MethodInfo methodInfo, object[] methodParams, AggregateException exception)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteAfter(object originalObject, MethodInfo methodInfo, object[] methodParams, Task methodResult)
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecuteAfter<T>(object originalObject, MethodInfo methodInfo, object[] methodParams, Task<T> methodResult)
        {
            throw new NotImplementedException();
        }
    }
}