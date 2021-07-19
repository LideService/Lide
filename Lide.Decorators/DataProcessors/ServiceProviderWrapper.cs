using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Lide.Decorators.ObjectDecorator;
using Lide.TracingProxy.Reflection;

namespace Lide.Decorators.DataProcessors
{
    public class ServiceProviderWrapper : IServiceProvider
    {
        private readonly Dictionary<object, object> _generatedProxies;
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _generatedProxies = new Dictionary<object, object>(new IdentityEqualityComparer<object>());
        }
        
        public object GetService(Type serviceType)
        {
            var originalObject = _serviceProvider.GetService(serviceType);
            if (originalObject == null)  
            {
                return null;
            }
            
            if (_generatedProxies.ContainsKey(originalObject))
            {
                return _generatedProxies[originalObject];
            }

            if (serviceType == typeof(HttpClient))
            {
                //
            }
            
            if (serviceType.IsInterface)
            {
                var proxy = ProxyDecoratorFactory.CreateProxyDecorator(serviceType);
                proxy.SetDecorator(new ConsoleDecorator());
                proxy.SetOriginalObject(originalObject);
                var decoratedObject = proxy.GetDecoratedObject();
                _generatedProxies.Add(originalObject, decoratedObject);
                return _generatedProxies[originalObject];
            }

            if (serviceType == typeof(IHttpClientFactory))
            {
                return new HttpClientFactoryWrapper(originalObject as IHttpClientFactory);
            }
            
            return _serviceProvider.GetService(serviceType);
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