using System;
using System.Net.Http;
using Lide.Core.Contract.Plugin;
using Lide.WebAPI.Contract;
using Lide.WebAPI.Wrappers;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebAPI.Plugin;

public class HttpClientFactoryPlugin : IServiceProviderPlugin
{
    private readonly Contract.IHttpClientBuilder _httpClientBuilder;

    public HttpClientFactoryPlugin(Contract.IHttpClientBuilder httpClientBuilder)
    {
        _httpClientBuilder = httpClientBuilder;
    }

    public Type Type => typeof(IHttpClientFactory);
    public bool ContinueDecoration => false;

    public object GetService(object originalObject)
    {
        return new HttpClientFactoryWrapper((IHttpClientFactory)originalObject, _httpClientBuilder);
    }
}