using System;
using System.Reflection;
using System.Threading.Tasks;
using Lide.AsyncProxy;

namespace Lide.WebApiTests
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            var obj = new T();
            var prox = DispatchProxyAsyncFactory.Create(typeof(IT), typeof(Test));
            ((Test)(object)prox).SetTarget(obj);
            obj.Do();
            ((IT)prox).Do();
            return;
            
        }

        // public static IHostBuilder CreateHostBuilder(string[] args) =>
        //     Host.CreateDefaultBuilder(args)
        //         .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
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


    public interface IT
    {
        void Do();
    }
    public class T : IT
    {
        public void Do()
        {
            Console.WriteLine("Do");
        }
    }
    
    public class Test : DispatchProxyAsync
    {
        private object _target;

        public void SetTarget(object target)
        {
            this._target = target;
        }

        public override object Invoke(MethodInfo methodInfo, object[] methodParameters)
        {
            Console.WriteLine("Invoke");
            return methodInfo.Invoke(_target, methodParameters);
        }

        public override Task InvokeAsync(MethodInfo methodInfo, object[] methodParameters)
        {
            Console.WriteLine("InvokeT");
            return (Task)methodInfo.Invoke(_target, methodParameters);
        }
        
        public override Task<T1> InvokeAsyncT<T1>(MethodInfo methodInfo, object[] methodParameters)
        {
            Console.WriteLine("InvokeT1");
            return (Task<T1>)methodInfo.Invoke(_target, methodParameters);
        }
    }
}