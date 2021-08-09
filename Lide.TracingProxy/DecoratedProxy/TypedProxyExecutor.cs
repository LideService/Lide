using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
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
            var modifiedMethodParameters = methodParameters;
            foreach (var proxyDecorator in _decorators)
            {
                ExecuteSafe(() => modifiedMethodParameters = proxyDecorator.ExecuteBefore(_originalObject, methodInfo, modifiedMethodParameters));
            }

            return modifiedMethodParameters;
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
                var modifiedException = exception;
                foreach (var proxyDecorator in _decorators)
                {
                    ExecuteSafe(() => modifiedException = proxyDecorator.ExecuteException(_originalObject, methodInfo, methodParameters, modifiedException));
                }

                ExceptionDispatchInfo.Capture(modifiedException).Throw();
                throw;
            }
        }

        private object ExecuteAfter(MethodInfo methodInfo, object[] methodParameters, object result)
        {
            foreach (var proxyDecorator in _decorators)
            {
                if (result is VoidReturn)
                {
                    ExecuteSafe(() => proxyDecorator.ExecuteAfter(_originalObject, methodInfo, methodParameters));
                }
                else
                {
                    ExecuteSafe(() => result = proxyDecorator.ExecuteAfter(_originalObject, methodInfo, methodParameters, result));
                }
            }

            return result;
        }

        private Task ExecuteAfterAsync(MethodInfo methodInfo, object[] methodParameters, Task resultTask)
        {
            foreach (var proxyDecorator in _decorators)
            {
                ExecuteSafe(() => proxyDecorator.ExecuteAfter(_originalObject, methodInfo, methodParameters, resultTask));
            }

            var wrappedTask = resultTask?.ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    var aggregateException = task.Exception;
                    foreach (var proxyDecorator in _decorators)
                    {
                        ExecuteSafe(() => aggregateException = proxyDecorator.ExecuteException(_originalObject, methodInfo, methodParameters, aggregateException));
                    }

                    ExceptionDispatchInfo.Capture(aggregateException).Throw();
                }
                else
                {
                    foreach (var proxyDecorator in _decorators)
                    {
                        ExecuteSafe(() => proxyDecorator.ExecuteAfter(_originalObject, methodInfo, methodParameters));
                    }
                }
            });

            return wrappedTask;
        }

        private Task<TReturnType> ExecuteAfterAsync<TReturnType>(MethodInfo methodInfo, object[] methodParameters, Task<TReturnType> resultTask)
        {
            foreach (var proxyDecorator in _decorators)
            {
                ExecuteSafe(() => proxyDecorator.ExecuteAfter(_originalObject, methodInfo, methodParameters, resultTask));
            }

            var wrappedTask = resultTask?.ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    var aggregateException = task.Exception;
                    foreach (var proxyDecorator in _decorators)
                    {
                        ExecuteSafe(() => aggregateException = proxyDecorator.ExecuteException(_originalObject, methodInfo, methodParameters, aggregateException));
                    }

                    ExceptionDispatchInfo.Capture(aggregateException).Throw();
                    throw new Exception();
                }

                var result = task.Result;
                foreach (var proxyDecorator in _decorators)
                {
                    ExecuteSafe(() => result = proxyDecorator.ExecuteAfter(_originalObject, methodInfo, methodParameters, result));
                }

                return result;
            });

            return wrappedTask;
        }

        private void ExecuteSafe(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                // ignored
            }
        }
    }
}
