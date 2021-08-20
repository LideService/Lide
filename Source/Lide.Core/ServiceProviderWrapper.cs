using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Lide.Core.Contract.Plugin;
using Lide.Core.Contract.Provider;
using Lide.TracingProxy;

namespace Lide.Core
{
    public class ServiceProviderWrapper : IServiceProvider
    {
        private readonly Dictionary<object, object> _generatedProxies;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDecoratorContainer _decoratorContainer;
        private readonly ISettingsProvider _settingsProvider;
        private readonly List<IServiceProviderPlugin> _plugins;

        public ServiceProviderWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _decoratorContainer = (IDecoratorContainer)serviceProvider.GetService(typeof(IDecoratorContainer)) ?? throw new Exception($"Missing service type {nameof(IDecoratorContainer)}");
            _settingsProvider = (ISettingsProvider)_serviceProvider.GetService(typeof(ISettingsProvider)) ?? throw new Exception($"Missing service type {nameof(ISettingsProvider)}");
            _plugins = ((IEnumerable<IServiceProviderPlugin>)_serviceProvider.GetService(typeof(IEnumerable<IServiceProviderPlugin>)))?.ToList() ?? new List<IServiceProviderPlugin>();
            _generatedProxies = new Dictionary<object, object>(new IdentityEqualityComparer<object>());
        }

        public object GetService(Type serviceType)
        {
            var originalObject = _serviceProvider.GetService(serviceType);
            if (originalObject == null)
            {
                return null;
            }

            if (IsTypeExcluded(serviceType))
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
                var decoratedObject = plugin.GetService(originalObject);
                if (decoratedObject != null)
                {
                    _generatedProxies.Add(originalObject, decoratedObject);
                    return decoratedObject;
                }
            }

            if (serviceType.IsInterface)
            {
                var proxy = ProxyDecoratorFactory.CreateProxyDecorator(serviceType);
                proxy.SetDecorators(_decoratorContainer.GetDecorators());
                proxy.SetOriginalObject(originalObject);
                var decoratedObject = proxy.GetDecoratedObject();
                _generatedProxies.Add(originalObject, decoratedObject);
                return decoratedObject;
            }

            return originalObject;
        }

        private bool IsTypeExcluded(Type serviceType)
        {
            var assemblyName = serviceType.Assembly.GetName().Name;
            var @namespace = serviceType.Namespace;
            var excludedByType = _settingsProvider.ExcludedTypes.Any(serviceType.Name.StartsWith);
            var excludedByNamespace = @namespace != null && _settingsProvider.ExcludedNamespaces.Any(@namespace.StartsWith);
            var excludedByAssembly = assemblyName != null && _settingsProvider.ExcludedAssemblies.Any(assemblyName.StartsWith);
            return excludedByType || excludedByNamespace || excludedByAssembly;
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