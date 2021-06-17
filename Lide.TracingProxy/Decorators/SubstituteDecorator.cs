using System;
using System.Reflection;
using Lide.TracingProxy.Contracts;
using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.Decorators
{
    public class SubstituteDecorator : IProxyDecorator
    {
        public DecoratorType DecoratorType => DecoratorType.Subtitute;

        public void ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
        }

        public T ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, T methodResult)
        {
            return methodResult;
        }

        public object[] ExecuteBefore(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
            return methodParams;
        }

        public Exception ExecuteException(MethodInfo methodInfo, object originalObject, object[] methodParams, Exception exception)
        {
            return exception;
        }
    }
}