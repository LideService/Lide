using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.Contract
{
    [SuppressMessage("Stylecop", "SA1502", Justification = "Readability")]
    public interface IObjectDecorator
    {
        /// <summary>
        /// Name of the decorator to be identified with (From request header/body/query).
        /// Must be unique across all existing IObjectDecorators.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///  Indicates if the decorator is readonly or not.
        /// <returns>true - if all of {methodInfo}, {methodParams}, {methodResult}, {originalObject} or {exception} remain unchanged at all times</returns>
        /// <returns>false - if any modification is applied to  </returns>
        /// </summary>
        bool IsVolatile { get; }

        /// <summary>
        /// Called before the invocation of the methodInfo.
        /// </summary>
        /// <param name="plainObject">The object on which the methodInfo will be executed.</param>
        /// <param name="methodInfo">The method signature to be/is executed.</param>
        /// <param name="originalParameters">Non edited parameters designated to be passed to the method.</param>
        /// <param name="editedParameters">Possibly edited parameters by prior volatile decorators. Will match the last volatile ExecuteBeforeInvoke output.</param>
        /// <returns>
        /// Desired parameters to be passed to the method - works only if the decorator is volatile.
        /// If no volatile decorators are used, the originalParameters will be passed onto the method.
        /// </returns>
        object[] ExecuteBeforeInvoke(object plainObject, MethodInfo methodInfo, object[] originalParameters, object[] editedParameters)
        {
            return originalParameters;
        }

        /// <summary>
        /// Called after the invocation of the methodInfo is completed.
        /// If return type is Task or Task{T} its called after the task completes.
        /// If return type is void, Task or exception is raised within the method, Result will be null.
        /// If return type is value or Task{T} and no exception occured, Result probably wont be null.
        /// </summary>
        /// <param name="plainObject">the object on which the methodInfo will be executed.</param>
        /// <param name="methodInfo">the method signature to be/is executed.</param>
        /// <param name="originalParameters">params to be passed to the method.</param>
        /// <param name="originalEorR">Non edited exception or result after the method is completed.</param>
        /// <param name="editedEorR">Possibly edited parameters by prior volatile decorators. Will match the last volatile ExecuteBeforeInvoke output.</param>
        /// <returns>
        /// Desired method result behavior.
        /// Works only if the decorator is volatile.
        /// If `Exception` is set, the method result, if any, will be replaced with thrown `Exception`.
        /// If `Exception` is null, and the return type is value or Task{T}, `Result` will be used as return value regardless of what happened.
        /// If no volatile decorators are used, the original `Result` will be return, or `Exception` raised.
        /// </returns>
        ExceptionOrResult ExecuteAfterResult(object plainObject, MethodInfo methodInfo, object[] originalParameters, ExceptionOrResult originalEorR, ExceptionOrResult editedEorR)
        {
            return originalEorR;
        }
    }
}