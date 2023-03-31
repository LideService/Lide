using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Lide.Demo.Reporting.WebAPI;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Configuration.GetBuilder();
        builder.Configure(app => app.ConfigureApp());
        var app = builder.Build();
        await app.RunAsync().ConfigureAwait(false);
    }
}
