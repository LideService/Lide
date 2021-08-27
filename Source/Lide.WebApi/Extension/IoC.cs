using Lide.Core;
using Lide.Core.Contract;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Plugin;
using Lide.Core.Contract.Provider;
using Lide.Core.Facade;
using Lide.Core.Model;
using Lide.Core.Model.Settings;
using Lide.Core.Provider;
using Lide.Decorators;
using Lide.TracingProxy.Contract;
using Lide.WebApi.Contract;
using Lide.WebApi.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebApi.Extension
{
    public static class IoC
    {
        public static void AddLideCore(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Facades
            serviceCollection.AddSingleton<IConsoleFacade, ConsoleFacade>();
            serviceCollection.AddSingleton<IDateTimeFacade, DateTimeFacade>();
            serviceCollection.AddSingleton<IGuidFacade, GuidFacade>();
            serviceCollection.AddSingleton<ILoggerFacade, LoggerFacade>();
            serviceCollection.AddSingleton<IRandomFacade, RandomFacade>();
            serviceCollection.AddSingleton<ISerializerFacade, SerializerFacade>();

            // Core
            serviceCollection.AddSingleton<IFileWriter, FileWriter>();
            serviceCollection.AddSingleton<ITaskRunner, TaskRunner>();

            // Provider
            serviceCollection.AddSingleton<ICompressionProvider, CompressionProvider>();
            serviceCollection.AddSingleton<IFileNameProvider, FileNameProvider>();
            serviceCollection.AddSingleton<IParametersSerializer, MethodParamsSerializer>();
            serviceCollection.AddSingleton<ISignatureProvider, SignatureProvider>();

            // Decorators
            serviceCollection.AddScoped<IObjectDecoratorReadonly, ConsoleDecorator>();
            serviceCollection.AddScoped<IObjectDecoratorReadonly, DiagnosticsDecorator>();

            // Scoped?
            serviceCollection.AddScoped<IScopeIdProvider, ScopeIdProvider>();
            serviceCollection.AddScoped<ISettingsProvider, SettingsProvider>();

            // Web
            serviceCollection.AddScoped<IServiceProviderPlugin, HttpClientFactoryPlugin>();
            serviceCollection.AddScoped<IServiceProviderPlugin, HttpClientPlugin>();
            serviceCollection.AddScoped<IHttpHeaderProcessor, HttpHeaderProcessor>();

            serviceCollection.Configure<AppSettings>(configuration.GetSection("LideSettings"));
        }
    }
}