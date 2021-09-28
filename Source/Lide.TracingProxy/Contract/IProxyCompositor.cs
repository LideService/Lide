using System;
using System.Collections.Generic;
using Lide.Core.Contract.Provider;

namespace Lide.TracingProxy.Contract
{
    public interface IProxyCompositor<TInterface>
        where TInterface : class
    {
        IProxyCompositor<TInterface> SetOriginalObject(TInterface originalObject);
        IProxyCompositor<TInterface> SetDecorators(IEnumerable<IObjectDecoratorReadonly> readonlyDecorators);
        IProxyCompositor<TInterface> SetDecorator(IObjectDecoratorReadonly readonlyDecorator);
        IProxyCompositor<TInterface> SetDecorators(IEnumerable<IObjectDecoratorVolatile> volatileDecorators);
        IProxyCompositor<TInterface> SetDecorator(IObjectDecoratorVolatile volatileDecorator);
        IProxyCompositor<TInterface> SetActivatorProvider(IActivatorProvider activatorProvider);
        IProxyCompositor<TInterface> SetLogErrorAction(Action<string> logError);
        TInterface GetDecoratedObject();
    }
}