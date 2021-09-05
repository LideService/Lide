using System;
using System.Text;
using Lide.Core.Facade;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TaxCalculator.WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // var filePath = "/home/nikola.gamzakov/substitute2";
            // var fileFacade = new FileFacade();
            // var data1 = "Something1";
            // var data2 = "Something else 2";
            // var bytes1 = Encoding.UTF8.GetBytes(data1);
            // var bytes2 = Encoding.UTF8.GetBytes(data2);
            // fileFacade.WriteToFile(filePath, bytes1).Wait();
            // fileFacade.WriteToFile(filePath, bytes2).Wait();
            // Console.WriteLine(bytes1.Length);
            // Console.WriteLine(bytes2.Length);
            // var rbytes1 = fileFacade.ReadNextBatch(filePath, 0).Result;
            // var rdata1 = Encoding.UTF8.GetString(rbytes1);
            // Console.WriteLine(rdata1);
            // var pos = fileFacade.GetEndPosition(rbytes1, 0);
            // Console.WriteLine(pos);
            // var rbytes2 = fileFacade.ReadNextBatch(filePath, pos).Result;
            // var rdata2 = Encoding.UTF8.GetString(rbytes2);
            // Console.WriteLine(rdata2);
            // return;
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}