using System;
using System.Reflection;
using System.Threading.Tasks;
using Lide.TracingProxy.Contract;

namespace Lide.Decorators.ObjectDecorator
{
    public class ConsoleDecorator : IObjectDecorator
    {
        public string Id { get; } = "Console";

        public object[] ExecuteBefore(MethodInfo methodInfo, object[] methodParams) 
        {
            Console.WriteLine("object[] ExecuteBefore");
            return methodParams;
        }

        public void ExecuteAfter(MethodInfo methodInfo, object[] methodParams)
        {
            Console.WriteLine("void ExecuteAfter");
        }

        public T ExecuteAfter<T>(MethodInfo methodInfo, object[] methodParams, T methodResult)
        {
            Console.WriteLine("T ExecuteAfter<T>");
            return methodResult;
        }

        public Exception ExecuteException(MethodInfo methodInfo, object[] methodParams, Exception exception)
        {
            Console.WriteLine("Exception ExecuteException");
            return exception;
        }

        public AggregateException ExecuteException(MethodInfo methodInfo, object[] methodParams, AggregateException exception)
        {
            Console.WriteLine("AggregateException ExecuteException");
            return exception;
        }

        public Task ExecuteAfter(MethodInfo methodInfo, object[] methodParams, Task methodResult)
        {
            Console.WriteLine("Task ExecuteAfter");
            return methodResult;
        }

        public Task<T> ExecuteAfter<T>(MethodInfo methodInfo, object[] methodParams, Task<T> methodResult)
        {
            Console.WriteLine("Task<T> ExecuteAfter<T>");
            return methodResult;
        }
    }
}