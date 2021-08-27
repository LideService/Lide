using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TaxCalculator.WebApi
{
    public interface IYieldTest
    {
        List<int> Get2();
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            // var o = new YieldTest();
            // var c = new ConsoleDecorator(new ConsoleFacade(), new SignatureProvider(), new SerializerFacade(), new ScopeIdProvider(new DateTimeFacade(), new RandomFacade()), new TaskRunner());
            // var decorator = ProxyDecoratorFactory.CreateProxyDecorator(typeof(IYieldTest));
            // decorator.SetDecorators(new List<IObjectDecoratorReadonly>(){ c });
            // decorator.SetOriginalObject(o);
            // var d = (IYieldTest)decorator.GetDecoratedObject();
            // d.Get2();
            // Console.Read();

            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public class YieldTest : IYieldTest
        {
            public List<int> Get2()
            {
                return new List<int>() { 1, 2, 3 };
            }
        }
    }
}