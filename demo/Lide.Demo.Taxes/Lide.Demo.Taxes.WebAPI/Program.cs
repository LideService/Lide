using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Lide.Demo.Taxes.WebAPI;

internal static class Program
{
    private static async Task Main()
    {
        var builder = Configuration.GetBuilder();
        builder.Configure(app => app.ConfigureApp());
        var app = builder.Build();
        await app.RunAsync().ConfigureAwait(false);
    }
}
