using System;
using System.Reflection;
using System.Threading.Tasks;
using Lide.AsyncProxy;
using Lide.TracingProxy.ObjectDecorator;
using Lide.TracingProxy.Reflection;

namespace Lide.WebApiTests
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            var obj = new T();
            var prox = DispatchProxyAsyncFactory.Create(typeof(IT), typeof(Test));
            var prox2 = ProxyDecoratorFactory.CreateProxyDecorator<IT>();
            var prox3 = ProxyDecoratorFactory.CreateProxyDecorator(typeof(IT));
            prox2.SetDecorator(new ConsoleDecorator());
            prox2.SetOriginalObject(obj);
            prox3.SetDecorator(new ConsoleDecorator());
            prox3.SetDecorator(new ConsoleDecorator());
            prox3.SetOriginalObject(obj);
            ((Test)(object)prox).SetTarget(obj);
            obj.Do();
            ((IT)prox).Do();
            prox2.GetDecoratedObject().Do();
            ((IT)prox3.GetDecoratedObject()).Do();
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