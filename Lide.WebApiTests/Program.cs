using System;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Lide.WebApiTests
{
    public static class Program
    {
        
        public static async Task Main(string[] args)
        {
            var a = new object[] {1, 7, 5};
            var b = new object[] {1, 7, 5};
            Console.WriteLine(a.GetHashCode());
            Console.WriteLine(b.GetHashCode());
            
            
            return;
            // var a = new Task(() =>
            // {
            //     Console.WriteLine("Test");
            //     return;
            // });
            // Console.WriteLine("1");
            // var b = Newtonsoft.Json.JsonConvert.SerializeObject(a);
            // Console.WriteLine("2");
            // var c = Newtonsoft.Json.JsonConvert.DeserializeObject<Task>(b);
            // Console.WriteLine("3");
            // Console.WriteLine(b);
            // c.Start();
            // return;
            // Do(1,"", 3);
            //
            //
            //
            // return;
            // CreateHostBuilder(args).Build().Run();
            // return;
        }

        public static void Do(int a, string b, double c)
        {
            string s = null;


            try
            {
                var e = 0;
                var d = (int)7 / (int)e;
            }
            catch (Exception e)
            {
                s = Newtonsoft.Json.JsonConvert.SerializeObject(e);
                Console.WriteLine(s);
                Console.WriteLine();
                Console.WriteLine(e);
                Console.WriteLine();
                //throw;
                //ExceptionDispatchInfo.Capture(e).Throw();
            }

            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.All,
                Context = new StreamingContext(StreamingContextStates.CrossAppDomain),
            };
            var e1 = Newtonsoft.Json.JsonConvert.DeserializeObject<DivideByZeroException>(s,settings);
            ExceptionDispatchInfo.Capture(e1).Throw();
            Console.WriteLine(a);
            Console.WriteLine(b);
            Console.WriteLine(c);
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}