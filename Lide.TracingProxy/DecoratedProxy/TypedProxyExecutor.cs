using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.DecoratedProxy
{
    [SuppressMessage("Exception", "CA1031", Justification = "Never throws for any reason")]
    [SuppressMessage("ReSharper", "UnusedTypeParameter", Justification = "Partial class")]
    public partial class ProxyDecoratorTyped<TInterface>
        where TInterface : class
    {
        private object[] ExecuteBefore(MethodInfo methodInfo, object[] methodParameters)
        {
            var editedParameters = methodParameters;
            foreach (var proxyDecorator in _decorators)
            {
                try
                {
                    var returnParameters = proxyDecorator.ExecuteBeforeInvoke(_originalObject, methodInfo, methodParameters, editedParameters);
                    if (proxyDecorator.IsVolatile)
                    {
                        editedParameters = returnParameters;
                    }
                }
                catch (Exception e)
                {
                    _logError?.Invoke($"Decorator {proxyDecorator.Id} has encountered an error with action {nameof(proxyDecorator.ExecuteBeforeInvoke)}: {e}");
                }
            }

            return editedParameters;
        }

        private TReturnType ExecuteMethodInfo<TReturnType>(MethodInfo methodInfo, object[] methodParameters)
        {
            try
            {
                var executableMethodInfo = _methodInfoCache.GetOrAdd(_originalObjectType, methodInfo, () => _methodInfoProvider.GetMethodInfoCompiled(methodInfo));
                return (TReturnType)executableMethodInfo.Invoke(_originalObject, methodParameters);
            }
            catch (Exception exception)
            {
                var editedEorR = ExecuteAfterDecorators(methodInfo, methodParameters, exception, null);
                return (TReturnType)editedEorR.Result;
            }
        }

        private object ExecuteAfter(MethodInfo methodInfo, object[] methodParameters, object result)
        {
            var editedEorR = ExecuteAfterDecorators(methodInfo, methodParameters, null, result is VoidReturn ? null : result);
            return result is VoidReturn ? null : editedEorR.Result;
        }

        private Task ExecuteAfterAsync(MethodInfo methodInfo, object[] methodParameters, Task resultTask)
        {
            return resultTask?.ContinueWith(task => ExecuteAfterDecorators(methodInfo, methodParameters, task.Exception, null));
        }

        private Task<TReturnType> ExecuteAfterAsync<TReturnType>(MethodInfo methodInfo, object[] methodParameters, Task<TReturnType> resultTask)
        {
            return resultTask?.ContinueWith(task =>
            {
                var editedEorR = ExecuteAfterDecorators(methodInfo, methodParameters, task.Exception, task.Result);
                return (TReturnType)editedEorR.Result;
            });
        }

        private ExceptionOrResult ExecuteAfterDecorators(MethodInfo methodInfo, object[] methodParameters, Exception exception, object result)
        {
            var originalEorR = new ExceptionOrResult(exception, result);
            var editedEorR = new ExceptionOrResult(exception, result);
            foreach (var proxyDecorator in _decorators)
            {
                try
                {
                    var resultEorR = proxyDecorator.ExecuteAfterResult(_originalObject, methodInfo, methodParameters, originalEorR, editedEorR);
                    if (proxyDecorator.IsVolatile)
                    {
                        editedEorR = resultEorR;
                    }
                }
                catch (Exception e)
                {
                    _logError?.Invoke($"Decorator {proxyDecorator.Id} has encountered an error with action {nameof(proxyDecorator.ExecuteAfterResult)}: {e}");
                }
            }

            if (editedEorR.Exception != null)
            {
                ExceptionDispatchInfo.Capture((AggregateException) editedEorR.Exception).Throw();
                throw new Exception();
            }

            return editedEorR;
        }
    }
}
