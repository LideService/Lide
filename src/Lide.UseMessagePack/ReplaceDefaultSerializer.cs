using System.Linq;
using Lide.Core.Contract.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.UseMessagePack;

public static class ReplaceDefaultSerializer
{
    public static void AddMessagePack(this IServiceCollection serviceCollection)
    {
        var binarySerializer = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IBinarySerializeProvider));
        if (binarySerializer != null)
        {
            serviceCollection.Remove(binarySerializer);
        }

        serviceCollection.AddSingleton<IBinarySerializeProvider, MessagePackAsBinarySerializer>();
    }
}