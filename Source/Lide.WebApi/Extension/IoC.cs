using Lide.Core.Contract;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Plugin;
using Lide.Core.Contract.Provider;
using Lide.Core.Facade;
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
            serviceCollection.AddSingleton<IDateTimeFacade, DateTimeFacade>();
            serviceCollection.AddSingleton<ILoggerFacade, LoggerFacade>();
            serviceCollection.AddSingleton<IRandomFacade, RandomFacade>();
            serviceCollection.AddSingleton<ISerializerFacade, SerializerFacade>();
            serviceCollection.AddSingleton<IFileFacade, FileFacade>();
            serviceCollection.AddSingleton<IPathFacade, PathFacade>();

            // Provider
            serviceCollection.AddSingleton<ICompressionProvider, CompressionProvider>();
            serviceCollection.AddSingleton<IParametersSerializer, MethodParamsSerializer>();
            serviceCollection.AddSingleton<ISignatureProvider, SignatureProvider>();

            // Decorators
            serviceCollection.AddScoped<IObjectDecoratorReadonly, ConsoleDecorator>();
            serviceCollection.AddScoped<IObjectDecoratorReadonly, DiagnosticsDecorator>();
            serviceCollection.AddScoped<IObjectDecoratorReadonly, SubstituteRecordDecorator>();
            serviceCollection.AddScoped<IObjectDecoratorVolatile, SubstituteReplayDecorator>();

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