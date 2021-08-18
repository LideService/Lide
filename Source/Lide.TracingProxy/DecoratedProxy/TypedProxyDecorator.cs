using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Lide.AsyncProxy;
using Lide.TracingProxy.Contract;

namespace Lide.TracingProxy.DecoratedProxy
{
    public partial class ProxyDecoratorTyped<TInterface> : DispatchProxyAsync
        where TInterface : class
    {
        private readonly List<IObjectDecorator> _decorators = new ();
        private IMethodInfoCache _methodInfoCache;
        private IMethodInfoProvider _methodInfoProvider;
        private TInterface _originalObject;
        private Type _originalObjectType;
        private Action<string> _logError;

        public override object Invoke(MethodInfo methodInfo, object[] originalParameters)
        {
            var editedParameters = ExecuteBefore(methodInfo, originalParameters);
            var result = ExecuteMethodInfo<object>(methodInfo, originalParameters, editedParameters);
            result = ExecuteAfter(methodInfo, originalParameters, editedParameters, result);
            return result;
        }

        public override Task InvokeAsync(MethodInfo methodInfo, object[] originalParameters)
        {
            var editedParameters = ExecuteBefore(methodInfo, originalParameters);
            var result = ExecuteMethodInfo<Task>(methodInfo, originalParameters, editedParameters);
            result = ExecuteAfterAsync(methodInfo, originalParameters, editedParameters, result);
            return result;
        }

        public override Task<TReturnType> InvokeAsyncT<TReturnType>(MethodInfo methodInfo, object[] originalParameters)
        {
            var editedParameters = ExecuteBefore(methodInfo, originalParameters);
            var result = ExecuteMethodInfo<Task<TReturnType>>(methodInfo, originalParameters, editedParameters);
            result = ExecuteAfterAsync(methodInfo, originalParameters, editedParameters, result);
            return result;
        }
    }
}
