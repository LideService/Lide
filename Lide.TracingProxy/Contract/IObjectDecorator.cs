using System;
using System.Reflection;

namespace Lide.TracingProxy.Contract
{
    public interface IObjectDecorator
    {
        /// <param name="methodInfo"></param>
        /// <param name="originalObject"></param>
        /// <param name="methodParams">the params to be passed to the method</param>
        /// <returns>methodParams - you can change the params passed if needed</returns>
        object[] ExecuteBefore(MethodInfo methodInfo, object originalObject, object[] methodParams);
        void ExecuteAfter(MethodInfo methodInfo, object originalObject, object[] methodParams);

        /// <param name="methodInfo"></param>
        /// <param name="originalObject"></param>
        /// <param name="methodParams"></param>
        /// <param name="methodResult">the result of the execution of the method</param>
        /// <returns>methodResult - you can replace the returned result if needed</returns>
        T ExecuteAfter<T>(MethodInfo methodInfo, object originalObject, object[] methodParams, T methodResult);
        
        /// <param name="methodInfo"></param>
        /// <param name="originalObject"></param>
        /// <param name="methodParams"></param>
        /// <param name="exception">the exception of the execution of the method</param>
        /// <returns>exception - you can replace the exception if needed</returns>
        Exception ExecuteException(MethodInfo methodInfo, object originalObject, object[] methodParams, Exception exception);
    }
}