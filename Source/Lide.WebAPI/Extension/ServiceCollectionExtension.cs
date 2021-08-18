using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Plugin;
using Lide.Core.Contract.Provider;
using Lide.Core.Facade;
using Lide.Core.Provider;
using Lide.WebAPI.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebAPI.Extension
{
    public static class IoC
    {
        public static void AddLideCore(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IActivatorFacade, ActivatorFacade>();
            serviceCollection.AddSingleton<IConsoleFacade, ConsoleFacade>();
            serviceCollection.AddSingleton<IFileWriter, FileWriter>();
            serviceCollection.AddSingleton<IGuidFacade, GuidFacade>();
            serviceCollection.AddSingleton<ILoggerFacade, LoggerFacade>();
            serviceCollection.AddSingleton<ISerializerFacade, SerializerFacade>();
            
            serviceCollection.AddSingleton<IAssemblyPreloader, AssemblyPreloader>();
            serviceCollection.AddSingleton<ICompressionProvider, CompressionProvider>();
            serviceCollection.AddSingleton<IDecoratorContainer, DecoratorContainer>();
            serviceCollection.AddSingleton<IFileNameProvider, FileNameProvider>();
            serviceCollection.AddSingleton<IMethodParamsSerializer, MethodParamsSerializer>();
            serviceCollection.AddSingleton<IScopeProvider, ScopeProvider>();
            serviceCollection.AddSingleton<ISettingsProvider, SettingsProvider>();
            serviceCollection.AddSingleton<ISignatureProvider, SignatureProvider>();
            
            serviceCollection.AddSingleton<IServiceProviderPlugin, HttpClientFactoryPlugin>();
            serviceCollection.AddSingleton<IServiceProviderPlugin, HttpClientPlugin>();
        }
    }
}