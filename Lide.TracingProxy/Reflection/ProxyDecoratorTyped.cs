using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Lide.AsyncProxy;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.DataProcessors.Contract;
using Lide.TracingProxy.Reflection.Contract;
using Lide.TracingProxy.Reflection.Model;

namespace Lide.TracingProxy.Reflection
{
    public partial class ProxyDecoratorTyped<TInterface> : DispatchProxyAsync
        where TInterface : class
    {
        private readonly List<IObjectDecorator> _decorators = new ();
        private IMethodInfoCache _methodInfoCache;
        private IMethodInfoProvider _methodInfoProvider;
        private IScopeTracker _scopeTracker;
        private TInterface _originalObject;
        private Type _originalObjectType;

        public override object Invoke(MethodInfo methodInfo, object[] methodParameters)
        {
            ExecuteBefore(methodInfo, methodParameters);
            var result = ExecuteSafe(methodInfo, methodParameters, ExecuteAfter);
            return result;
        }

        public override Task InvokeAsync(MethodInfo methodInfo, object[] methodParameters)
        {
            ExecuteBefore(methodInfo, methodParameters);
            var result = ExecuteSafe(methodInfo, methodParameters, ExecuteAfterAsync);
            return result;
        }

        public override Task<TReturnType> InvokeAsyncT<TReturnType>(MethodInfo methodInfo, object[] methodParameters)
        {
            ExecuteBefore(methodInfo, methodParameters);
            var result = ExecuteSafe(methodInfo, methodParameters, ExecuteAfterAsync<TReturnType>);
            return result;
        }

        private void ExecuteBefore(MethodInfo methodInfo, object[] methodParameters)
        {
            foreach (var proxyDecorator in _decorators)
            {
                methodParameters = proxyDecorator.ExecuteBefore(methodInfo, _originalObject, methodParameters);
            }
        }

        private object ExecuteAfter(MethodInfo methodInfo, object[] methodParameters)
        {
            var executableMethodInfo = GetExecutableMethodInfo(methodInfo);
            var result = executableMethodInfo.Invoke(_originalObject, methodParameters);
            foreach (var proxyDecorator in _decorators)
            {
                if (result is VoidReturn)
                {
                    proxyDecorator.ExecuteAfter(methodInfo, _originalObject, methodParameters);
                }
                else
                {
                    result = proxyDecorator.ExecuteAfter(methodInfo, _originalObject, methodParameters, result);
                }
            }

            return result;
        }

        private Task ExecuteAfterAsync(MethodInfo methodInfo, object[] methodParameters)
        {
            var executableMethodInfo = GetExecutableMethodInfo(methodInfo);
            var resultTask = (Task)executableMethodInfo.Invoke(_originalObject, methodParameters);
            var wrappedTask = resultTask?.ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    foreach (var proxyDecorator in _decorators)
                    {
                        proxyDecorator.ExecuteException(methodInfo, _originalObject, methodParameters, task.Exception);
                    }
                }
                else
                {
                    foreach (var proxyDecorator in _decorators)
                    {
                        proxyDecorator.ExecuteAfter(methodInfo, _originalObject, methodParameters);
                    }
                }
            });

            return wrappedTask;
        }

        private Task<TReturnType> ExecuteAfterAsync<TReturnType>(MethodInfo methodInfo, object[] methodParameters)
        {
            var executableMethodInfo = GetExecutableMethodInfo(methodInfo);
            var resultTask = (Task<TReturnType>)executableMethodInfo.Invoke(_originalObject, methodParameters);
            var wrappedTask = resultTask?.ContinueWith(task =>
            {
                TReturnType result = task.Result;
                if (task.Exception != null)
                {
                    foreach (var proxyDecorator in _decorators)
                    {
                        proxyDecorator.ExecuteException(methodInfo, _originalObject, methodParameters, task.Exception);
                    }
                }
                else
                {
                    foreach (var proxyDecorator in _decorators)
                    {
                        result = proxyDecorator.ExecuteAfter(methodInfo, _originalObject, methodParameters, result);
                    }
                }

                return result;
            });

            return wrappedTask;
        }

        private MethodInfoCompiled GetExecutableMethodInfo(MethodInfo methodInfo)
        {
            if (_methodInfoCache.Exists(_originalObjectType, methodInfo))
            {
                return _methodInfoCache.GetValue(_originalObjectType, methodInfo);
            }

            var methodInfoDelegate = _methodInfoProvider.GetMethodInfoCompiled(methodInfo);
            _methodInfoCache.TryAdd(_originalObjectType, methodInfo, methodInfoDelegate);
            return methodInfoDelegate;
        }

        private TReturnType ExecuteSafe<TReturnType>(MethodInfo methodInfo, object[] methodParameters, Func<MethodInfo, object[], TReturnType> executeAction)
        {
            try
            {
                return executeAction(methodInfo, methodParameters);
            }
            catch (Exception exception)
            {
                foreach (var proxyDecorator in _decorators)
                {
                    exception = proxyDecorator.ExecuteException(methodInfo, _originalObject, methodParameters, exception);
                }

                ExceptionDispatchInfo.Capture(exception).Throw();
                throw;
            }
        }
    }
}
