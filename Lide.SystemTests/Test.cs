using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Lide.TracingProxy.Model;

namespace T
{
    public interface ITest
    {
        string ToDO { get; set; }
        void D1();
        string D2();
        Task D3();
        Task<string> D4();
    }

    public class Test : ITest
    {
        public string ToDO { get; set; } = "Something";

        public void D1()
        {
            Console.WriteLine("D1");
        }

        public string D2()
        {
            return "D2";
        }

        public Task D3()
        {
            return Task.CompletedTask;
        }

        public Task<string> D4()
        {
            return Task.FromResult("Trying");
        }
    }

    public interface IProxyDecorator
    {
        DecoratorType DecoratorType { get; }
        object[] ExecuteBefore(MethodInfo methodInfo, object originalObject, object[] methodParams);
        void ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams);
        T ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, T methodResult);

        Exception ExecuteException(MethodInfo methodInfo, object originalObject, object[] methodParams,
            Exception exception);
    }

    public class ConsoleDecorator : IProxyDecorator
    {
        public DecoratorType DecoratorType { get; } = DecoratorType.Logging;

        public object[] ExecuteBefore(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
            Console.WriteLine("Before");
            Console.WriteLine(originalObject.GetType());
            Console.WriteLine(methodInfo);
            Console.WriteLine(JsonSerializer.Serialize(methodParams));
            return methodParams;
        }

        public void ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
            Console.WriteLine("After");
            Console.WriteLine(originalObject.GetType());
            Console.WriteLine(methodInfo);
            Console.WriteLine(JsonSerializer.Serialize(methodParams));
        }

        public T ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, T methodResult)
        {
            Console.WriteLine("AfterT");
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

    public class LoggingDecorator<T> : DispatchProxy
    {
        private T _originalObject;

        private readonly List<IProxyDecorator> _decorators = new()
        {
            new ConsoleDecorator()
        };


        private readonly MethodInfo GenericTask;

        public LoggingDecorator()
        {
            GenericTask = this.GetType().BaseType.GetRuntimeMethods().Where(x => x.Name == nameof(HandleTaskGenericAsync)).First();
        }

        private Task<T2> HandleTaskGenericAsync<T1, T2>(T originalObject, T1 result, MethodInfo methodInfo,
            object[] methodParameters)
            where T1 : Task<T2>
        {
            return result.ContinueWith(parent =>
            {
                var endResult = parent.Result;
                foreach (var proxyDecorator in _decorators)
                {
                    endResult = proxyDecorator.ExecuteAfter(methodInfo, originalObject, methodParameters,
                        parent.Result);
                }

                return endResult;
            });
        }

        protected override object Invoke(MethodInfo methodInfo, object[] methodParameters)
        {
            foreach (var proxyDecorator in _decorators)
            {
                methodParameters = proxyDecorator.ExecuteBefore(methodInfo, _originalObject, methodParameters);
            }

            var result = methodInfo.Invoke(_originalObject, methodParameters);
            if (result is Task task)
            {
                var resultType = result.GetType();
                var returnType = resultType.GetGenericArguments().First();
                return GenericTask.MakeGenericMethod(resultType, returnType).Invoke(this,
                    new object[] {_originalObject, task, methodInfo, methodParameters});
            }

            foreach (var proxyDecorator in _decorators)
            {
                result = proxyDecorator.ExecuteAfter(methodInfo, _originalObject, methodParameters, result);
            }

            return result;
        }

        public static T Create(T decorated)
        {
            object proxy = Create<T, LoggingDecorator<T>>();
            ((LoggingDecorator<T>) proxy)._originalObject = decorated;

            return (T) proxy;
        }
    }
}