using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Core.Contract.Provider;
using Lide.Core.Provider;
using Lide.TracingProxy.Contract;

namespace Lide.TracingProxy.DecoratedProxy;

public partial class ProxyDecorator<TInterface> : IProxyCompositor<TInterface>
    where TInterface : class
{
    public IProxyCompositor<TInterface> SetOriginalObject(TInterface originalObject, bool singleton = false)
    {
        _originalObject = originalObject;
        _isSingleton = singleton;
        return this;
    }

    public IProxyCompositor<TInterface> SetDecorators(IEnumerable<IObjectDecoratorReadonly> readonlyDecorators)
    {
        _readonlyDecorators.AddRange(readonlyDecorators.Where(x => x != null));
        return this;
    }

    public IProxyCompositor<TInterface> SetDecorator(IObjectDecoratorReadonly readonlyDecorator)
    {
        if (readonlyDecorator != null)
        {
            _readonlyDecorators.Add(readonlyDecorator);
        }

        return this;
    }

    public IProxyCompositor<TInterface> SetDecorators(IEnumerable<IObjectDecoratorVolatile> volatileDecorators)
    {
        _volatileDecorators.AddRange(volatileDecorators.Where(x => x != null));
        return this;
    }

    public IProxyCompositor<TInterface> SetDecorator(IObjectDecoratorVolatile volatileDecorator)
    {
        if (volatileDecorator != null)
        {
            _volatileDecorators.Add(volatileDecorator);
        }

        return this;
    }

    public IProxyCompositor<TInterface> SetActivatorProvider(IActivatorProvider activatorProvider)
    {
        _activatorProvider = activatorProvider;
        return this;
    }

    public IProxyCompositor<TInterface> SetLogErrorAction(Action<string> logError)
    {
        _logError = logError;
        return this;
    }

    public TInterface GetDecoratedObject()
    {
        if (_originalObject == null || (_readonlyDecorators.Count == 0 && _volatileDecorators.Count == 0))
        {
            return _originalObject;
        }

        _activatorProvider ??= new ActivatorProvider();
        return (TInterface)(object)this;
    }
}