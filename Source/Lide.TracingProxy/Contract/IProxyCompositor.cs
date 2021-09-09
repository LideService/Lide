using System;
using System.Collections.Generic;

namespace Lide.TracingProxy.Contract
{
    public interface IProxyCompositor<TInterface>
        where TInterface : class
    {
        IProxyCompositor<TInterface> SetOriginalObject(TInterface originalObject);
        IProxyCompositor<TInterface> SetDecorators(IEnumerable<IObjectDecoratorReadonly> readonlyDecorators);
        IProxyCompositor<TInterface> SetDecorators(IEnumerable<IObjectDecoratorVolatile> volatileDecorators);
        IProxyCompositor<TInterface> SetLogErrorAction(Action<string> logError);
        TInterface GetDecoratedObject();
    }
}