using Lide.Core;
using Lide.Core.Contract;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Plugin;
using Lide.Core.Contract.Provider;
using Lide.Core.Facade;
using Lide.Core.Model;
using Lide.Core.Provider;
using Lide.Decorators;
using Lide.WebApi.Contract;
using Lide.WebApi.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebApi.Extension
{
    public static class IoC
    {
        private static ConsoleDecorator T { get; }

        public static void AddLideCore(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IActivatorFacade, ActivatorFacade>();
            serviceCollection.AddSingleton<IConsoleFacade, ConsoleFacade>();
            serviceCollection.AddSingleton<IDateTimeFacade, DateTimeFacade>();
            serviceCollection.AddSingleton<IGuidFacade, GuidFacade>();
            serviceCollection.AddSingleton<ILoggerFacade, LoggerFacade>();
            serviceCollection.AddSingleton<IRandomFacade, RandomFacade>();
            serviceCollection.AddSingleton<ISerializerFacade, SerializerFacade>();

            serviceCollection.AddSingleton<IFileWriter, FileWriter>();
            serviceCollection.AddSingleton<ITaskRunner, TaskRunner>();

            serviceCollection.AddSingleton<IAssemblyPreloader, AssemblyPreloader>();
            serviceCollection.AddSingleton<ICompressionProvider, CompressionProvider>();
            serviceCollection.AddSingleton<IDecoratorContainer, DecoratorContainer>();
            serviceCollection.AddSingleton<IFileNameProvider, FileNameProvider>();
            serviceCollection.AddSingleton<IParametersSerializer, MethodParamsSerializer>();
            serviceCollection.AddSingleton<ISignatureProvider, SignatureProvider>();

            serviceCollection.AddSingleton<IScopeProvider, ScopeProvider>();
            serviceCollection.AddSingleton<ISettingsProvider, SettingsProvider>();

            serviceCollection.AddSingleton<IServiceProviderPlugin, HttpClientFactoryPlugin>();
            serviceCollection.AddSingleton<IServiceProviderPlugin, HttpClientPlugin>();

            serviceCollection.AddSingleton<IHttpHeaderProcessor, HttpHeaderProcessor>();

            serviceCollection.Configure<LideAppSettings>(configuration.GetSection("LideAppSettings"));
        }
    }
}