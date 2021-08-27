using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Lide.Core.Contract.Plugin;
using Lide.Core.Contract.Provider;
using Lide.TracingProxy;
using Lide.TracingProxy.Contract;

namespace Lide.Core
{
    public class ServiceProviderWrapper : IServiceProvider, IDisposable
    {
        private readonly Dictionary<object, object> _generatedProxies;
        private readonly IServiceProvider _serviceProvider;
        private readonly Action _disposeProvider;
        private readonly List<IServiceProviderPlugin> _plugins;
        private readonly List<IObjectDecoratorReadonly> _readonlyDecorators;
        private readonly List<IObjectDecoratorVolatile> _volatileDecorators;

        // Might break with scoped
        public ServiceProviderWrapper(IServiceProvider scoped, Action disposeProvider)
        {
            _serviceProvider = scoped;
            _disposeProvider = disposeProvider;
            SettingsProvider = (ISettingsProvider)scoped.GetService(typeof(ISettingsProvider)) ?? throw new Exception($"Missing service type {nameof(ISettingsProvider)}");
            ScopeIdProvider = (IScopeIdProvider)scoped.GetService(typeof(IScopeIdProvider)) ?? throw new Exception($"Missing service type {nameof(IScopeIdProvider)}");
            _plugins = ((IEnumerable<IServiceProviderPlugin>)scoped.GetService(typeof(IEnumerable<IServiceProviderPlugin>)))?.ToList() ?? new List<IServiceProviderPlugin>();
            _readonlyDecorators = ((IEnumerable<IObjectDecoratorReadonly>)scoped.GetService(typeof(IEnumerable<IObjectDecoratorReadonly>)))?.ToList() ?? new List<IObjectDecoratorReadonly>();
            _volatileDecorators = ((IEnumerable<IObjectDecoratorVolatile>)scoped.GetService(typeof(IEnumerable<IObjectDecoratorVolatile>)))?.ToList() ?? new List<IObjectDecoratorVolatile>();
            _generatedProxies = new Dictionary<object, object>(new IdentityEqualityComparer<object>());
        }

        public ISettingsProvider SettingsProvider { get; private set; }
        public IScopeIdProvider ScopeIdProvider { get; private set; }

        public void Dispose()
        {
            _disposeProvider?.Invoke();
            GC.SuppressFinalize(this);
        }

        public object GetService(Type serviceType)
        {
            var originalObject = _serviceProvider.GetService(serviceType);
            if (originalObject == null)
            {
                return null;
            }

            if (!SettingsProvider.AllowEnablingDecorators)
            {
                return originalObject;
            }

            if (!SettingsProvider.IsTypeAllowed(serviceType))
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
            var proxy = ProxyDecoratorFactory.CreateProxyDecorator(serviceType);
            proxy.SetDecorators(_readonlyDecorators.Where(x => SettingsProvider.GetDecorators().Contains(x.Id)));
            proxy.SetDecorators(_volatileDecorators.Where(x => SettingsProvider.AllowVolatileDecorators && SettingsProvider.GetDecorators().Contains(x.Id)));
            proxy.SetOriginalObject(originalObject);
            var decoratedObject = proxy.GetDecoratedObject();
            return decoratedObject;
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