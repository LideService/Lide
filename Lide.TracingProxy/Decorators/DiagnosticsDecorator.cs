using System;
using System.Reflection;
using Lide.TracingProxy.Contracts;
using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.Decorators
{
    public class DiagnosticsDecorator : IProxyDecorator
    {
        public DecoratorType DecoratorType => DecoratorType.Diagnostics;

        public void ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
            throw new NotImplementedException();
        }

        public T ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, T methodResult)
        {
            throw new NotImplementedException();
        }

        public object[] ExecuteBefore(MethodInfo methodInfo, object originalObject, object[] methodParams)
        {
            throw new NotImplementedException();
        }

        public Exception ExecuteException(MethodInfo methodInfo, object originalObject, object[] methodParams, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}