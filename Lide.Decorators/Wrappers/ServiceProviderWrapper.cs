using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Lide.Decorators.Contract;
using Lide.Decorators.ObjectDecorator;
using Lide.TracingProxy;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.Decorators.Wrappers
{
    public class ServiceProviderWrapper : IServiceProvider
    {
        private readonly Dictionary<object, object> _generatedProxies;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDecoratorContainer _decoratorContainer;
        private readonly IHttpHeaderProcessor _httpHeaderProcessor;
        private readonly ILideSettingsProcessor _lideSettingsProcessor;

        public ServiceProviderWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _decoratorContainer = (IDecoratorContainer)serviceProvider.GetRequiredService(typeof(IDecoratorContainer));
            _httpHeaderProcessor = (IHttpHeaderProcessor)_serviceProvider.GetRequiredService(typeof(IHttpHeaderProcessor));
            _lideSettingsProcessor = (ILideSettingsProcessor) _serviceProvider.GetRequiredService(typeof(ILideSettingsProcessor));
            _generatedProxies = new Dictionary<object, object>(new IdentityEqualityComparer<object>());
        }
        
        public object GetService(Type serviceType)
        {
            var originalObject = _serviceProvider.GetService(serviceType);
            if (originalObject == null)  
            {
                return null;
            }

            var assemblyName = serviceType.Assembly.GetName().Name;
            var @namespace = serviceType.Namespace;
            var excludedByType = _lideSettingsProcessor.ExcludedTypes.Contains(serviceType.Name);
            var excludedByNamespace =  @namespace != null && _lideSettingsProcessor.ExcludedNamespaces.Contains(@namespace);
            var excludedByAssembly = assemblyName != null && _lideSettingsProcessor.ExcludedAssemblies.Contains(assemblyName);
            if (excludedByType || excludedByNamespace || excludedByAssembly)
            {
                return originalObject;
            }
            
            if (_generatedProxies.ContainsKey(originalObject))
            {
                return _generatedProxies[originalObject];
            }

            if (originalObject is HttpClient httpClient)
            {
                httpClient.DefaultRequestHeaders.Add("","");
                _httpHeaderProcessor!.AddHeaders(httpClient);
            }
            
            if (serviceType == typeof(IHttpClientFactory))
            {
                var wrapper = new HttpClientFactoryWrapper(originalObject as IHttpClientFactory, _httpHeaderProcessor);
                _generatedProxies.Add(originalObject, wrapper);
                return wrapper;
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