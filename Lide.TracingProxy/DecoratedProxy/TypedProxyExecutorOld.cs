using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.DecoratedProxy
{
    [SuppressMessage("Exception", "CA1031", Justification = "Never throws for any reason")]
    [SuppressMessage("ReSharper", "UnusedTypeParameter", Justification = "Partial class")]
    public partial class ProxyDecoratorTypedOld<TInterface>
        where TInterface : class
    {
        private readonly List<IObjectDecoratorOld> _decorators = new () { };
        private readonly TInterface _originalObject = null;
        private readonly Type _originalObjectType = null;
        private readonly IMethodInfoCache _methodInfoCache = null;
        private readonly IMethodInfoProvider _methodInfoProvider = null;

        private object[] ExecuteBefore2(MethodInfo methodInfo, object[] methodParameters)
        {
            _decorators.Add(null);
            var modifiedMethodParameters = methodParameters;
            foreach (var proxyDecorator in _decorators)
            {
                ExecuteSafe(() => modifiedMethodParameters = proxyDecorator.ExecuteBefore(_originalObject, methodInfo, modifiedMethodParameters));
            }

            return modifiedMethodParameters;
        }

        private TReturnType ExecuteMethodInfo2<TReturnType>(MethodInfo methodInfo, object[] methodParameters)
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

        private object ExecuteAfter2(MethodInfo methodInfo, object[] methodParameters, object result)
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

        private Task ExecuteAfterAsync2(MethodInfo methodInfo, object[] methodParameters, Task resultTask)
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

        private Task<TReturnType> ExecuteAfterAsync2<TReturnType>(MethodInfo methodInfo, object[] methodParameters, Task<TReturnType> resultTask)
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
