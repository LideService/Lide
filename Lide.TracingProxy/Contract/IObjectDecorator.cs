using System;
using System.Reflection;

namespace Lide.TracingProxy.Contract
{
    public interface IObjectDecorator
    {
        object[] ExecuteBefore(MethodInfo methodInfo, object originalObject, object[] methodParams);
        void ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams);
        T ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, T methodResult);
        Exception ExecuteException(MethodInfo methodInfo, object originalObject, object[] methodParams, Exception exception);
    }
}