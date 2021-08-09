using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Lide.TracingProxy.Contract
{
    public interface IObjectDecorator
    {
        /// <summary>
        /// Name of the decorator to be identified with (From request header/body/query).
        /// Must be unique across all existing IObjectDecorators.
        /// Note the interface provides default implementations for all methods that are non volatile.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///  Indicates if the decorator is readonly or not.
        /// <returns>true - if all of {methodInfo}, {methodParams}, {methodResult}, {originalObject} or {exception} remain unchanged at all times</returns>
        /// <returns>false - if any modification is applied to  </returns>
        /// </summary>
        bool Volatile { get; }

        /// <summary>
        /// called before the invocation of the methodInfo.
        /// </summary>
        /// <param name="originalObject">the object on which the methodInfo will be executed.</param>
        /// <param name="methodInfo">the method signature to be/is executed.</param>
        /// <param name="methodParams">params to be passed to the method..</param>
        /// <returns>methodParams - allows for the params to be changed/replaced if needed.</returns>
        object[] ExecuteBefore(object originalObject, MethodInfo methodInfo, object[] methodParams)
        {
            return methodParams;
        }

        /// <summary>
        /// called after the invocation of the methodInfo is completed
        /// called if the return type is `void`, or `Task` and the task is completed.
        /// </summary>
        /// <param name="originalObject">the object on which the methodInfo will be executed.</param>
        /// <param name="methodInfo">the method signature to be/is executed.</param>
        /// <param name="methodParams">params to be passed to the method..</param>
        void ExecuteAfter(object originalObject, MethodInfo methodInfo, object[] methodParams)
        {
        }

        /// <summary>
        /// called after the invocation of the methodInfo is completed.
        /// called if the return type is `value`, or `Task`T` and the task is completed.
        /// </summary>
        /// <param name="originalObject">the object on which the methodInfo will be executed.</param>
        /// <param name="methodInfo">the method signature to be/is executed.</param>
        /// <param name="methodParams">params to be passed to the method.</param>
        /// <param name="methodResult">result of the execution in case of `value` or `Task`T` return type.</param>
        /// <returns>methodResult - allows for the result to be changed/replaced if needed.</returns>
        /// <typeparam name="T">the type of `value` or the `T` in `Task`T`.</typeparam>
        T ExecuteAfter<T>(object originalObject, MethodInfo methodInfo, object[] methodParams, T methodResult)
        {
            return methodResult;
        }

        /// <summary>
        /// called after the invocation of the methodInfo is completed.
        /// called if the method have thrown an exception.
        /// </summary>
        /// <param name="originalObject">the object on which the methodInfo will be executed.</param>
        /// <param name="methodInfo">the method signature to be/is executed.</param>
        /// <param name="methodParams">params to be passed to the method.</param>
        /// <param name="exception">exception thrown from the method, or wrapped in the return task.</param>
        /// <returns>exception - allows for the exception to be changed/replaced if needed.</returns>
        Exception ExecuteException(object originalObject, MethodInfo methodInfo, object[] methodParams, Exception exception)
        {
            return exception;
        }

        /// <summary>
        /// called after the invocation of the methodInfo is completed.
        /// called if the method has return type of `Task` or `Task`T` and the completed task has aggregate exception.
        /// </summary>
        /// <param name="originalObject">the object on which the methodInfo will be executed.</param>
        /// <param name="methodInfo">the method signature to be/is executed.</param>
        /// <param name="methodParams">params to be passed to the method.</param>
        /// <param name="exception">exception thrown from the method, or wrapped in the return task.</param>
        /// <returns>exception - allows for the exception to be changed/replaced if needed.</returns>
        AggregateException ExecuteException(object originalObject, MethodInfo methodInfo, object[] methodParams, AggregateException exception)
        {
            return exception;
        }

        /// <summary>
        /// called after the invocation of the methodInfo is completed.
        /// called if the return type is `Task`.
        /// </summary>
        /// <warning>
        /// no insurance the task is complete at this point.
        /// be ware if you await the task as it can compromise the process.
        /// if you need to access the task after completion, use `.ContinueWith`.
        /// or refer to `void ExecuteAfter` above.
        /// </warning>
        /// <param name="originalObject">the object on which the methodInfo will be executed.</param>
        /// <param name="methodInfo">the method signature to be/is executed.</param>
        /// <param name="methodParams">params to be passed to the method.</param>
        /// <param name="methodResult">result of the execution as `Task`.</param>
        /// <returns>methodResult - allows for the task to be changed, replaced, appended, awaited if needed.</returns>
        Task ExecuteAfter(object originalObject, MethodInfo methodInfo, object[] methodParams, Task methodResult)
        {
            return methodResult;
        }

        /// <summary>
        /// called after the invocation of the methodInfo is completed.
        /// called if the return type is `Task`T`.
        /// </summary>
        /// <warning>
        /// no insurance the task is complete at this point.
        /// be ware if you await the task as it can compromise the process.
        /// if you need to access the task after completion, use `.ContinueWith`.
        /// or refer to `T ExecuteAfter`T` above.
        /// </warning>
        /// <param name="originalObject">the object on which the methodInfo will be executed.</param>
        /// <param name="methodInfo">the method signature to be/is executed.</param>
        /// <param name="methodParams">params to be passed to the method.</param>
        /// <param name="methodResult">result of the execution as `Task`T`.</param>
        /// <returns>methodResult - allows for the task to be changed, replaced, appended, awaited if needed.</returns>
        Task<T> ExecuteAfter<T>(object originalObject, MethodInfo methodInfo, object[] methodParams, Task<T> methodResult)
        {
            return methodResult;
        }
    }
}