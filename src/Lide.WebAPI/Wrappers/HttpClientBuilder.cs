using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Lide.Core.Contract.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebAPI.Wrappers;

public class HttpClientBuilder : Contract.IHttpClientBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IActivatorProvider _activatorProvider;

    public HttpClientBuilder(
        IServiceProvider serviceProvider,
        IActivatorProvider activatorProvider)
    {
        _serviceProvider = serviceProvider;
        _activatorProvider = activatorProvider;
    }

    public HttpClient RebuildNewClient(HttpClient originalClient)
    {
        var handlerInfo = typeof(HttpMessageInvoker)
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .First(x => x.Name == "_handler");
        var handler = (HttpMessageHandler)handlerInfo.GetValue(originalClient);
        var newHandler = (HttpMessageHandler)_activatorProvider.CreateObject(handler!.GetType());
        var newClient = new HttpClient(newHandler);
        _activatorProvider.DeepCopyIntoExistingObject(originalClient, newClient);

        var wrappedHandler = ActivatorUtilities.CreateInstance(_serviceProvider, typeof(HttpMessageHandlerWrapper), newHandler);
        handlerInfo.SetValue(newClient, wrappedHandler);
        return newClient;
    }
}