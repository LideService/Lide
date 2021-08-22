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
    public class ServiceProviderWrapper : IServiceProvider
    {
        private readonly Dictionary<object, object> _generatedProxies;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly List<IServiceProviderPlugin> _plugins;
        private readonly List<IObjectDecorator> _decorators;

        public ServiceProviderWrapper(IServiceProvider serviceProvider, IServiceProvider scoped)
        {
            _serviceProvider = serviceProvider;
            _settingsProvider = (ISettingsProvider)scoped.GetService(typeof(ISettingsProvider)) ?? throw new Exception($"Missing service type {nameof(ISettingsProvider)}");
            _plugins = ((IEnumerable<IServiceProviderPlugin>)scoped.GetService(typeof(IEnumerable<IServiceProviderPlugin>)))?.ToList() ?? new List<IServiceProviderPlugin>();
            _decorators = ((IEnumerable<IObjectDecorator>)scoped.GetService(typeof(IEnumerable<IObjectDecorator>)))?.ToList() ?? new List<IObjectDecorator>();
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
                proxy.SetDecorators(GetDecorators().ToArray());
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

        private IEnumerable<IObjectDecorator> GetDecorators()
        {
            return _decorators
                .Where(x => _settingsProvider.AppliedDecorators.Contains(x.Id))
                .Where(x => !x.IsVolatile || _settingsProvider.AllowVolatileDecorators);
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