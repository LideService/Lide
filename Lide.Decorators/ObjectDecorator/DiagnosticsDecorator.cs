using System;
using System.Reflection;

namespace Lide.TracingProxy.ObjectDecorator
{
    public class DiagnosticsDecorator
    {
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