using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Plugin;
using Lide.Core.Contract.Provider;
using Lide.Core.Provider;
using Lide.TracingProxy;
using Lide.TracingProxy.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebApi.Wrappers
{
    public class ServiceProviderWrapper : IServiceProvider, IDisposable
    {
        private static readonly ConcurrentDictionary<Type, bool> UnsupportedTypes = new ();
        private readonly Dictionary<object, object> _generatedProxies;
        private readonly IServiceProvider _serviceProvider;
        private readonly Action _disposeProvider;
        private readonly List<IServiceProviderPlugin> _plugins;
        private readonly List<IObjectDecoratorReadonly> _readonlyDecorators;
        private readonly List<IObjectDecoratorVolatile> _volatileDecorators;
        private readonly ILoggerFacade _loggerFacade;

        public ServiceProviderWrapper(IServiceProvider scoped, Action disposeProvider)
        {
            _serviceProvider = scoped;
            _disposeProvider = disposeProvider;
            SettingsProvider = (ISettingsProvider)scoped.GetService(typeof(ISettingsProvider)) ?? throw new Exception($"Missing service type {nameof(ISettingsProvider)}");
            ScopeIdProvider = (IScopeIdProvider)scoped.GetService(typeof(IScopeIdProvider)) ?? throw new Exception($"Missing service type {nameof(IScopeIdProvider)}");
            _loggerFacade = (ILoggerFacade)scoped.GetService(typeof(ILoggerFacade)) ?? throw new Exception($"Missing service type {nameof(ILoggerFacade)}");
            _plugins = ((IEnumerable<IServiceProviderPlugin>)scoped.GetService(typeof(IEnumerable<IServiceProviderPlugin>)))?.ToList() ?? new List<IServiceProviderPlugin>();
            _readonlyDecorators = ((IEnumerable<IObjectDecoratorReadonly>)scoped.GetService(typeof(IEnumerable<IObjectDecoratorReadonly>)))?.ToList() ?? new List<IObjectDecoratorReadonly>();
            _volatileDecorators = ((IEnumerable<IObjectDecoratorVolatile>)scoped.GetService(typeof(IEnumerable<IObjectDecoratorVolatile>)))?.ToList() ?? new List<IObjectDecoratorVolatile>();
            _generatedProxies = new Dictionary<object, object>(new IdentityEqualityComparer<object>());
        }

        public ISettingsProvider SettingsProvider { get; private set; }
        public IScopeIdProvider ScopeIdProvider { get; private set; }

        public void Dispose()
        {
            foreach (var proxy in _generatedProxies.Values)
            {
                if (proxy is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _disposeProvider?.Invoke();
            GC.SuppressFinalize(this);
        }

        public object GetService(Type serviceType)
        {
            var originalObject = _serviceProvider.GetService(serviceType);
            if (originalObject == null || UnsupportedTypes.ContainsKey(serviceType))
            {
                return originalObject;
            }

            if (_generatedProxies.ContainsKey(originalObject))
            {
                return _generatedProxies[originalObject];
            }

            var plugin = _plugins.FirstOrDefault(x => x.Type == serviceType);
            if (plugin != null)
            {
                var pluginObject = plugin.GetService(originalObject);
                if (plugin.ContinueDecoration && serviceType.IsInterface)
                {
                    pluginObject = DecorateObject(serviceType, pluginObject);
                }

                _generatedProxies.Add(originalObject, pluginObject);
                return pluginObject;
            }

            if (!serviceType.IsInterface)
            {
                return originalObject;
            }

            var decoratedObject = DecorateObject(serviceType, originalObject);
            _generatedProxies.Add(originalObject, decoratedObject);
            return decoratedObject;
        }

        private object DecorateObject(Type serviceType, object originalObject)
        {
            try
            {
                var decorators = SettingsProvider.GetDecorators(serviceType);
                var proxy = ProxyDecoratorFactory.CreateProxyDecorator(serviceType);
                proxy.SetDecorators(_readonlyDecorators.Where(x => SettingsProvider.AllowReadonlyDecorators && decorators.Contains(x.Id)));
                proxy.SetDecorators(_volatileDecorators.Where(x => SettingsProvider.AllowVolatileDecorators && decorators.Contains(x.Id)));
                proxy.SetOriginalObject(originalObject);
                proxy.SetLogErrorAction(_loggerFacade.LogError);
                var decoratedObject = proxy.GetDecoratedObject();
                return decoratedObject;
            }
            catch (Exception e)
            {
                _loggerFacade.LogError($"[Lide] encountered unsupported type {serviceType} {e}");
                UnsupportedTypes.TryAdd(serviceType, true);
                return originalObject;
            }
        }

        private class IdentityEqualityComparer<T> : IEqualityComparer<T>
            where T : class
        {
            public int GetHashCode(T value)
            {
                return RuntimeHelpers.GetHashCode(value);
            }

            public bool Equals(T left, T right)
            {
                return ReferenceEquals(left, right);
            }
        }
    }
}