using System;
using Microsoft.Extensions.DependencyInjection;

namespace Unity.Microsoft.DependencyInjection
{
    public class ServiceProviderFactory : IServiceProviderFactory<IServiceProvider>
    {
        public IServiceProvider CreateBuilder(IServiceCollection services)
        {
            Console.WriteLine("Factory2");
            return new ServiceProviderCusom(services.BuildServiceProvider());
        }

        public IServiceProvider CreateServiceProvider(IServiceProvider container)
        {
            Console.WriteLine("Factory1");
            return new ServiceProviderCusom(container);
        }
    }
}