using System;
using System.Reflection;
using System.Text.Json;
using Lide.TracingProxy.Contract;

namespace Lide.TracingProxy.ObjectDecorator
{
    public class ConsoleDecorator : IObjectDecorator
    {
        public object[] ExecuteBefore(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
            Console.WriteLine(originalObject.GetType());
            Console.WriteLine(methodInfo);
            Console.WriteLine(JsonSerializer.Serialize(methodParams));
            return methodParams;
        }

        public void ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
            Console.WriteLine(originalObject.GetType());
            Console.WriteLine(methodInfo);
            Console.WriteLine(JsonSerializer.Serialize(methodParams));
        }

        public T ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, T methodResult)
        {
            Console.WriteLine(originalObject.GetType());
            Console.WriteLine(methodInfo);
            Console.WriteLine(JsonSerializer.Serialize(methodParams));
            Console.WriteLine(JsonSerializer.Serialize(methodResult));
            return methodResult;
        }

        public Exception ExecuteException(MethodInfo methodInfo, object originalObject, object[] methodParams,
            Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}