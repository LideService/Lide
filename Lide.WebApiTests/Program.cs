using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unity.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lide.WebApiTests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new TestFactory())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        
    }

    public class TestFactory : IServiceProviderFactory<Container>
    {
        private Container _container;
        public TestFactory()
        {
            _container = new Container();
        }
        public Container CreateBuilder(IServiceCollection services)
        {
            Console.WriteLine("CreateBuilder");
            _container.AddServices(services);
            return _container;
        }

        public IServiceProvider CreateServiceProvider(Container containerBuilder)
        {
            Console.WriteLine("CreateServiceProvider");
            return _container.GetProvider();
        }
    }

    public class Container
    {
        private IServiceCollection _services;
        private IServiceProvider _provider;
        
        public void AddServices(IServiceCollection services)
        {
            _services = services;
            _provider = new TestProvider(_services);
        }

        public IServiceProvider GetProvider()
        {
            return _provider;
        }
    }

    public class TestProvider : IServiceProvider
    {
        private readonly IServiceCollection _services;

        public TestProvider(IServiceCollection services)
        {
            _services = services;
        }
        
        public object GetService(Type serviceType)
        {
            Console.WriteLine($"GetService {serviceType}");
            var result =  _services.BuildServiceProvider().GetService(serviceType);
            var a = (Microsoft.Extensions.Hosting.Internal.) result;
            return result;
        }
    }
}