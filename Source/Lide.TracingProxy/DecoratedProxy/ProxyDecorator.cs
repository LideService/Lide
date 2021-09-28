using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Lide.AsyncProxy;
using Lide.Core.Contract.Provider;
using Lide.TracingProxy.Contract;

namespace Lide.TracingProxy.DecoratedProxy
{
    public partial class ProxyDecorator<TInterface> : DispatchProxyAsync
        where TInterface : class
    {
        private readonly List<IObjectDecoratorReadonly> _readonlyDecorators = new ();
        private readonly List<IObjectDecoratorVolatile> _volatileDecorators = new ();
        private IActivatorProvider _activatorProvider;
        private TInterface _originalObject;
        private Type _originalObjectType;
        private Action<string> _logError;

        public override object Invoke(MethodInfo methodInfo, object[] originalParameters)
        {
            var executeBeforeMetadata = ExecuteBefore(methodInfo, originalParameters);
            var executeMetadata = ExecuteMethodInfo(executeBeforeMetadata);
            var result = ExecuteAfter(executeMetadata);
            return result;
        }

        public override Task InvokeAsync(MethodInfo methodInfo, object[] originalParameters)
        {
            var executeBeforeMetadata = ExecuteBefore(methodInfo, originalParameters);
            var executeMetadata = ExecuteMethodInfo(executeBeforeMetadata);
            var result = ExecuteAfterAsync(executeMetadata);
            return result;
        }

        public override Task<TReturnType> InvokeAsyncT<TReturnType>(MethodInfo methodInfo, object[] originalParameters)
        {
            var executeBeforeMetadata = ExecuteBefore(methodInfo, originalParameters);
            var executeMetadata = ExecuteMethodInfo(executeBeforeMetadata);
            var result = ExecuteAfterAsyncT<TReturnType>(executeMetadata);
            return result;
        }
    }
}
