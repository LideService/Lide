using System;
using System.Net.Http;
using Lide.Core.Contract.Plugin;
using Lide.WebApi.Contract;

namespace Lide.WebApi.Plugin
{
    public class HttpClientFactoryPlugin : IServiceProviderPlugin
    {
        private readonly IHttpHeaderProcessor _httpHeaderProcessor;

        public HttpClientFactoryPlugin(IHttpHeaderProcessor httpHeaderProcessor)
        {
            _httpHeaderProcessor = httpHeaderProcessor;
        }

        public Type Type => typeof(IHttpClientFactory);

        public object GetService(object originalObject)
        {
            return new HttpClientFactoryWrapper(originalObject as IHttpClientFactory, _httpHeaderProcessor);
        }
    }
}