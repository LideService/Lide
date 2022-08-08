using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
using Lide.WebApi.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebApi.Extension
{
    public static class AddServices
    {
        public static void AddLideCore(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IServiceCollection>(sp => serviceCollection);

            // Facades
            serviceCollection.AddSingleton<IDateTimeFacade, DateTimeFacade>();
            serviceCollection.AddSingleton<ILoggerFacade, LoggerFacade>();
            serviceCollection.AddSingleton<IFileFacade, FileFacade>();
            serviceCollection.AddSingleton<IPathFacade, PathFacade>();
            serviceCollection.AddSingleton<IRandomFacade, RandomFacade>();

            // Provider
            serviceCollection.AddSingleton<IActivatorProvider, ActivatorProvider>();
            serviceCollection.AddSingleton<ICompressionProvider, CompressionProvider>();
            serviceCollection.AddSingleton<IJsonSerializeProvider, JsonSerializeProvider>();
            serviceCollection.AddSingleton<ISettingsProvider, SettingsProvider>();
            serviceCollection.AddSingleton<ISignatureProvider, SignatureProvider>();
            serviceCollection.AddSingleton<IStreamBatchProvider, StreamBatchProvider>();
            serviceCollection.AddSingleton<ITaskRunner, TaskRunner>();

            // Decorators
            serviceCollection.AddScoped<IObjectDecoratorReadonly, ConsoleDecorator>();
            serviceCollection.AddScoped<IObjectDecoratorReadonly, DiagnosticsDecorator>();
            serviceCollection.AddScoped<IObjectDecoratorReadonly, SubstituteRecordDecorator>();
            serviceCollection.AddScoped<IObjectDecoratorVolatile, SubstituteReplayDecorator>();

            serviceCollection.AddScoped<ISettingsProvider, SettingsProvider>();
            serviceCollection.AddScoped<IScopeIdProvider, ScopeIdProvider>();
            serviceCollection.AddScoped<IPropagateContentHandler, PropagateContentHandler>();

            // Web
            serviceCollection.AddScoped<HttpClient>();
            serviceCollection.AddScoped<IHttpClientRebuilder, HttpClientRebuilder>();
            serviceCollection.AddScoped<IServiceProviderPlugin, HttpClientFactoryPlugin>();
            serviceCollection.AddScoped<IServiceProviderPlugin, HttpClientPlugin>();

            serviceCollection.Configure<AppSettings>(configuration.GetSection("LideSettings"));
        }

        public static void ReplaceImplementation<TIType, TType>(this IServiceCollection serviceCollection)
            where TType : class, TIType
            where TIType : class
        {
            var binarySerializer = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(TIType));
            if (binarySerializer != null)
            {
                serviceCollection.Remove(binarySerializer);
            }

            serviceCollection.AddSingleton<TIType, TType>();
        }
    }
}