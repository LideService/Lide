using Microsoft.Extensions.DependencyInjection;
using System;

namespace Unity.Microsoft.DependencyInjection
{
    public class ServiceProviderCusom : IServiceProvider 
    {
        private IServiceProvider _container;

        internal ServiceProviderCusom(IServiceProvider container)
        {
            Console.WriteLine("Juked10");
            _container = container;
        }


        public object GetService(Type serviceType)
        {
            Console.WriteLine("Juked");
            if (null == _container) 
                throw new ObjectDisposedException(nameof(IServiceProvider));

            try
            {
                return _container.GetService(serviceType);
            }
            catch  { /* Ignore */}

            return null;
        }
    }
}