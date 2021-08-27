using System;
using System.Collections.Generic;

namespace Lide.TracingProxy.Contract
{
    public interface IProxyCompositorTyped<TInterface>
        where TInterface : class
    {
        IProxyCompositorTyped<TInterface> SetOriginalObject(TInterface originalObject);
        IProxyCompositorTyped<TInterface> SetDecorators(IEnumerable<IObjectDecoratorReadonly> readonlyDecorators);
        IProxyCompositorTyped<TInterface> SetDecorators(IEnumerable<IObjectDecoratorVolatile> volatileDecorators);
        IProxyCompositorTyped<TInterface> SetDelegateMethodInfoCache(IMethodInfoCache methodInfoCache);
        IProxyCompositorTyped<TInterface> SetDelegateMethodInfoProvider(IMethodInfoProvider methodInfoProvider);
        IProxyCompositorTyped<TInterface> SetLogErrorAction(Action<string> logError);
        TInterface GetDecoratedObject();
    }
}