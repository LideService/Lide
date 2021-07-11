using System;
using System.Reflection;
using System.Threading.Tasks;
using Lide.TracingProxy.Contract;

namespace Lide.TracingProxy.ObjectDecorator
{
    public class ConsoleDecorator : IObjectDecorator
    {
        public object[] ExecuteBefore(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
            Console.WriteLine("ExecuteBefore");
            return methodParams;
        }

        public void ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
            Console.WriteLine("ExecuteAfter");
        }

        public T ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, T methodResult)
        {
            Console.WriteLine("ExecuteAfter`T");
            return methodResult;
        }

        public Exception ExecuteException(MethodInfo methodInfo, object originalObject, object[] methodParams, Exception exception)
        {
            Console.WriteLine("ExecuteException");
            return exception;
        }

        public Task ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams, Task methodResult)
        {
            Console.WriteLine("ExecuteAfter-Task");
            return methodResult;
        }

        public Task<T> ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, Task<T> methodResult)
        {
            Console.WriteLine("ExecuteAfter-Task`T");
            return methodResult;
        }
    }
}