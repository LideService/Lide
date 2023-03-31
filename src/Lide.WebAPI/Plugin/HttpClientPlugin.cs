using System;
using System.Net.Http;
using Lide.Core.Contract.Plugin;
using Lide.WebAPI.Contract;

namespace Lide.WebAPI.Plugin;

public class HttpClientPlugin : IServiceProviderPlugin
{
    private readonly IHttpClientBuilder _httpClientBuilder;

    public HttpClientPlugin(IHttpClientBuilder httpClientBuilder)
    {
        _httpClientBuilder = httpClientBuilder;
    }

    public Type Type => typeof(HttpClient);
    public bool ContinueDecoration => false;

    public object GetService(object originalObject)
    {
        var originalClient = (HttpClient)originalObject;
        var newClient = _httpClientBuilder.RebuildNewClient(originalClient);
        return newClient;
    }
}