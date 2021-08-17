using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebAPI
{
    public static class IoC
    {
        public static void AddLide(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICompression, StringCompressor>();
            serviceCollection.AddSingleton<IConsoleWrapper, ConsoleWrapper>();
            serviceCollection.AddSingleton<IDecoratorContainer, DecoratorContainer>();
            serviceCollection.AddSingleton<IHttpHeaderProcessor, HttpHeaderProcessor>();
            serviceCollection.AddSingleton<ILideSettingsProcessor, LideSettingsProcessor>();
            serviceCollection.AddSingleton<ISignatureCallerMethod, SignatureCallerMethod>();
            serviceCollection.AddSingleton<ISignatureMethodInfo, SignatureMethodInfo>();
            serviceCollection.AddSingleton<IParametersSerializer, ParametersSerializer>();
            serviceCollection.AddSingleton<IStringSerializer, StringSerializer>();
        }
    }
}