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
    public partial class ProxyDecoratorTyped<TOriginalObject> : DispatchProxyAsync
        where TOriginalObject : class
    {
        private readonly List<IObjectDecorator> _decorators = new ();
        private IFastMethodInfoCache _fastMethodInfoCache = null!;
        private IScopeTracker _scopeTracker = null!;
        private TOriginalObject _originalObject = null!;
        private Type _originalObjectType = null!;

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
            MethodInfoDelegate executableMethodInfo = _fastMethodInfoCache.GetCompiledMethodInfo(_originalObjectType, methodInfo);
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
            MethodInfoDelegate executableMethodInfo = _fastMethodInfoCache.GetCompiledMethodInfo(_originalObjectType, methodInfo);
            var resultTask = (Task)executableMethodInfo.Invoke(_originalObject, methodParameters);
            var wrappedTask = resultTask.ContinueWith(_ =>
            {
                foreach (var proxyDecorator in _decorators)
                {
                    proxyDecorator.ExecuteAfter(methodInfo, _originalObject, methodParameters);
                }
            });

            return wrappedTask;
        }

        private Task<TReturnType> ExecuteAfterAsync<TReturnType>(MethodInfo methodInfo, object[] methodParameters)
        {
            MethodInfoDelegate executableMethodInfo = _fastMethodInfoCache.GetCompiledMethodInfo(_originalObjectType, methodInfo);
            var resultTask = (Task<TReturnType>)executableMethodInfo.Invoke(_originalObject, methodParameters);
            var wrappedTask = resultTask.ContinueWith(task =>
            {
                TReturnType result = task.Result;
                foreach (var proxyDecorator in _decorators)
                {
                    result = proxyDecorator.ExecuteAfter(methodInfo, _originalObject, methodParameters, result);
                }

                return result;
            });

            return wrappedTask;
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
