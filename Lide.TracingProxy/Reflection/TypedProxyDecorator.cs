using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Lide.AsyncProxy;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Reflection.Contract;

namespace Lide.TracingProxy.Reflection
{
    public partial class ProxyDecoratorTyped<TInterface> : DispatchProxyAsync
        where TInterface : class
    {
        private readonly List<IObjectDecorator> _decorators = new ();
        private IMethodInfoCache _methodInfoCache;
        private IMethodInfoProvider _methodInfoProvider;
        private TInterface _originalObject;
        private Type _originalObjectType;

        public override object Invoke(MethodInfo methodInfo, object[] methodParameters)
        {
            methodParameters = ExecuteBefore(methodInfo, methodParameters);
            var result = ExecuteMethodInfo<object>(methodInfo, methodParameters);
            result = ExecuteAfter(methodInfo, methodParameters, result);
            return result;
        }

        public override Task InvokeAsync(MethodInfo methodInfo, object[] methodParameters)
        {
            methodParameters = ExecuteBefore(methodInfo, methodParameters);
            var result = ExecuteMethodInfo<Task>(methodInfo, methodParameters);
            result = ExecuteAfterAsync(methodInfo, methodParameters, result);
            return result;
        }

        public override Task<TReturnType> InvokeAsyncT<TReturnType>(MethodInfo methodInfo, object[] methodParameters)
        {
            methodParameters = ExecuteBefore(methodInfo, methodParameters);
            var result = ExecuteMethodInfo<Task<TReturnType>>(methodInfo, methodParameters);
            result = ExecuteAfterAsync(methodInfo, methodParameters, result);
            return result;
        }
    }
}
