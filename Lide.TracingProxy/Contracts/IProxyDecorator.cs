using System;
using System.Reflection;
using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.Contracts
{
    public interface IProxyDecorator
    {
        DecoratorType DecoratorType { get; }
        object[] ExecuteBefore(MethodInfo methodInfo, object originalObject, object[] methodParams);
        void ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams);
        T ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, T methodResult);
        Exception ExecuteException(MethodInfo methodInfo, object originalObject, object[] methodParams, Exception exception);
    }
}