using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Plugin;
using Lide.Core.Contract.Provider;
using Lide.TracingProxy;
using Lide.TracingProxy.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebApi.Wrappers
{
    public sealed class ServiceProviderWrapper : IServiceProvider, IAsyncDisposable
    {
        private static readonly ConcurrentDictionary<Type, bool> UnsupportedTypes = new ();
        private readonly Dictionary<object, object> _generatedProxies;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<IServiceProviderPlugin> _plugins;
        private readonly IEnumerable<IObjectDecoratorReadonly> _readonlyDecorators;
        private readonly IEnumerable<IObjectDecoratorVolatile> _volatileDecorators;
        private readonly IServiceCollection _serviceCollection;
        private readonly ILoggerFacade _loggerFacade;

        public ServiceProviderWrapper(IServiceProvider scoped)
        {
            _serviceProvider = scoped;
            SettingsProvider = GetService<ISettingsProvider>(scoped);
            PropagateContentHandler = GetService<IPropagateContentHandler>(scoped);
            BinarySerializeProvider = GetService<IBinarySerializeProvider>(scoped);
            CompressionProvider = GetService<ICompressionProvider>(scoped);
            _serviceCollection = GetService<IServiceCollection>(scoped);
            _loggerFacade = GetService<ILoggerFacade>(scoped);
            _plugins = GetService<IEnumerable<IServiceProviderPlugin>>(scoped, true, new List<IServiceProviderPlugin>());
            _readonlyDecorators = GetService<IEnumerable<IObjectDecoratorReadonly>>(scoped, true, new List<IObjectDecoratorReadonly>());
            _volatileDecorators = GetService<IEnumerable<IObjectDecoratorVolatile>>(scoped, true, new List<IObjectDecoratorVolatile>());
            _generatedProxies = new Dictionary<object, object>(new IdentityEqualityComparer<object>());
        }

        public ISettingsProvider SettingsProvider { get; private set; }
        public IPropagateContentHandler PropagateContentHandler { get; private set; }
        public IBinarySerializeProvider BinarySerializeProvider { get; private set; }
        public ICompressionProvider CompressionProvider { get; private set; }

        public async ValueTask DisposeAsync()
        {
            foreach (var proxy in _generatedProxies.Values)
            {
                if (proxy is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                if (proxy is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
            }
        }

        public object GetService(Type serviceType)
        {
            var plugin = _plugins.FirstOrDefault(x => x.Type == serviceType);
            if (plugin == null && IsTypeDisallowed(serviceType))
            {
                return _serviceProvider.GetService(serviceType);
            }

            var instanceObject = GetInstanceObject(serviceType);
            if (serviceType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(serviceType))
            {
                return instanceObject;
            }

            if (_generatedProxies.ContainsKey(instanceObject))
            {
                return _generatedProxies[instanceObject];
            }

            if (plugin != null)
            {
                var pluginObject = plugin.GetService(instanceObject);
                if (plugin.ContinueDecoration && serviceType.IsInterface)
                {
                    pluginObject = DecorateObject(serviceType, pluginObject);
                }

                _generatedProxies.Add(instanceObject, pluginObject);
                return pluginObject;
            }

            if (!serviceType.IsInterface)
            {
                return instanceObject;
            }

            var decoratedObject = DecorateObject(serviceType, instanceObject);
            _generatedProxies.Add(instanceObject, decoratedObject);
            return decoratedObject;
        }

        private object GetInstanceObject(Type serviceType)
        {
            if (IsSingleton(serviceType))
            {
                return _serviceProvider.GetService(serviceType);
            }

            var implementingTypes = _serviceCollection
                .Where(x => x.ServiceType == serviceType)
                .Select(x => x.ImplementationType)
                .ToList();

            if (implementingTypes.Count == 0)
            {
                return _serviceProvider.GetService(serviceType);
            }

            if (serviceType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(serviceType))
            {
                var listGenericType = serviceType.GetGenericArguments().First();
                var listType = typeof(List<>).MakeGenericType(listGenericType);
                var list = (IList)Activator.CreateInstance(listType);
                var services = implementingTypes.Select(GetService).ToList();
                services.ForEach(x => list!.Add(x));
                return list;
            }

            var (constructorInfo, paramTypes) = GetConstructorData(serviceType);
            if (paramTypes.Count == 0)
            {
                return _serviceProvider.GetService(serviceType);
            }

            var constructorObjects = paramTypes.Select(GetService).ToArray();
            var instanceObject = constructorInfo.Invoke(constructorObjects);
            return instanceObject;
        }

        private bool IsTypeDisallowed(Type serviceType)
        {
            var implementingType = _serviceCollection.FirstOrDefault(x => x.ServiceType == serviceType)?.ImplementationType;
            var implementingTypeList = _serviceCollection.FirstOrDefault(x => x.ServiceType == serviceType.GetGenericArguments().FirstOrDefault())?.ImplementationType;
            return UnsupportedTypes.ContainsKey(serviceType)
                   || SettingsProvider.IsTypeDisallowed(serviceType)
                   || (implementingType != null && UnsupportedTypes.ContainsKey(implementingType))
                   || (implementingTypeList != null && UnsupportedTypes.ContainsKey(implementingTypeList));
        }

        private bool IsSingleton(Type serviceType)
        {
            return _serviceCollection.FirstOrDefault(x => x.ServiceType == serviceType)?.Lifetime == ServiceLifetime.Singleton;
        }

        private (ConstructorInfo constructorInfo, List<Type> paramTypes) GetConstructorData(Type serviceType)
        {
            var implementingType = _serviceCollection.FirstOrDefault(x => x.ServiceType == serviceType)?.ImplementationType;

            var constructors = implementingType?.GetConstructors();
            if (constructors is not { Length: 1 })
            {
                return (null, new List<Type>());
            }

            var constructor = constructors.First();
            var constructorParamTypes = constructor.GetParameters().Select(x => x.ParameterType).ToList();
            return (constructor, constructorParamTypes);
        }

        private object DecorateObject(Type serviceType, object originalObject)
        {
            try
            {
                var isSingleton = IsSingleton(serviceType);
                var decorators = SettingsProvider.GetDecorators(serviceType);
                var proxy = ProxyDecoratorFactory.CreateProxyDecorator(serviceType);
                proxy.SetDecorators(_readonlyDecorators.Where(x => SettingsProvider.AllowReadonlyDecorators && decorators.Contains(x.Id)));
                proxy.SetDecorators(_volatileDecorators.Where(x => SettingsProvider.AllowVolatileDecorators && decorators.Contains(x.Id)));
                proxy.SetOriginalObject(originalObject, isSingleton);
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

        private static TIService GetService<TIService>(IServiceProvider scoped, bool useDefault = false, TIService defaultReturn = null)
            where TIService : class
        {
            return useDefault
                ? (TIService)scoped.GetService(typeof(TIService)) ?? defaultReturn
                : (TIService)scoped.GetService(typeof(TIService)) ?? throw new Exception($"Missing service type {typeof(TIService).Name}");
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