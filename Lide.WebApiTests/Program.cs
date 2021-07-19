using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static System.String;

namespace Lide.WebApiTests
{
    public static class Program
    {

        private class MethodData
        {
            public string Name { get; set; }
            public string ReturnType { get; set; }
            public string DeclaringType { get; set; }
            public List<string> GenericArguments = new();
            public List<(string, string)> MethodParameters = new();
        }
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Int,Double".GetHashCode());
            Console.WriteLine("Double,Int".GetHashCode());
            //Console.WriteLine(CreateUniqueName(methodInfo));
            //58225482
            //58225482

            //CreateHostBuilder(args).Build().Run();
            return;
        }


        private static MethodData NewMethod(MethodInfo methodInfo)
        {
            var methodData = new MethodData()
            {
                Name = methodInfo.Name,
                ReturnType = methodInfo.ReturnType.ToString(),
                DeclaringType = methodInfo.DeclaringType?.ToString(),
            };

            foreach (var genericArgument in methodInfo.GetGenericArguments())
            {
                methodData.GenericArguments.Add(genericArgument.BaseType?.ToString());
            }

            foreach (var methodParameter in methodInfo.GetParameters())
            {
                methodData.MethodParameters.Add((methodParameter.Name, methodParameter.ParameterType?.ToString()));
            }

            return methodData;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

    public class T<T3>
    {
        public List<int> Do(int a, int c, int b)
        {
            Console.WriteLine(a);
            Console.WriteLine(b);
            Console.WriteLine(c);
            return null;
        }
        
        public List<int> Do<T1>(int a, int b, T1 c)
        {
            Console.WriteLine(a);
            Console.WriteLine(b);
            Console.WriteLine(c);
            return null;
        }
    }

    public class ContainerBuilder : IServiceProvider
    {
        private readonly IServiceProvider _portfeil;

        public ContainerBuilder(IServiceProvider portfeil)
        {
            _portfeil = portfeil;
        }
        
        public object GetService(Type serviceType)
        {
            Console.WriteLine($"Resolved from here {serviceType}");
            return _portfeil.GetService(serviceType);
        }
    }
}