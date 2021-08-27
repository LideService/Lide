using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Core;
using Lide.Core.Facade;
using Lide.Core.Provider;
using Lide.Decorators;
using Lide.TracingProxy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TaxCalculator.WebApi
{
    public interface IYieldTest
    {
        IEnumerable<int> Get2();
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var a = new YieldTest();
            var console = new ConsoleDecorator(
                new ConsoleFacade(),
                new SignatureProvider(),
                new SerializerFacade(),
                new ScopeIdProvider(new DateTimeFacade(), new RandomFacade()),
                new TaskRunner());

            var decorator = ProxyDecoratorFactory.CreateProxyDecorator(typeof(IYieldTest));
            decorator.SetDecorator(console);
            decorator.SetOriginalObject(a);
            var b = (IYieldTest)decorator.GetDecoratedObject();

            foreach (var i in a.Get2())
            {
                Console.WriteLine($"A{i}");
            }

            var br = b.Get2().ToList();
            foreach (var i in br)
            {
                Console.WriteLine($"B{i}");
            }
            ////CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public class YieldTest : IYieldTest
        {
            public IEnumerable<int> Get2()
            {
                Console.WriteLine("G1");
                yield return 1;
                Console.WriteLine("G2");
                yield return 2;
                Console.WriteLine("G3");
                yield return 3;
                Console.WriteLine("G4");
                yield return 4;
                Console.WriteLine("G5");
                yield return 5;
            }
        }
    }
}