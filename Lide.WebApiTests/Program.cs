using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Lide.TracingProxy.Contract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lide.WebApiTests
{
    public static class Program
    {
        
        public static void Main(string[] args)
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IT1, T1>();
            collection.AddSingleton<IT2, T2>();
            var provider = collection.BuildServiceProvider();
            
            ((IT2)ActivatorUtilities.CreateInstance(provider, typeof(T2))).Do();
            return;
            CreateHostBuilder(args).Build().Run();
            return;
        }

        public interface IT1
        {
            void Do();
        }

        public interface IT2
        {
            void Do();
        }

        public class T1 : IT1
        {
            public void Do()
            {
                Console.WriteLine("Do1");
            }
        }
        public class T2 : IT2
        {
            private readonly IT1 _it1;

            public T2(IT1 it1, string nonvalid)
            {
                _it1 = it1;
            }
            public void Do()
            {
                _it1.Do();
                Console.WriteLine("Do2");
            }
        }
        
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}