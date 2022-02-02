using System;
using System.Net.Http;
using Lide.Core.Contract.Plugin;
using Lide.WebApi.Contract;
using Lide.WebApi.Wrappers;

namespace Lide.WebApi.Plugin
{
    public class HttpClientFactoryPlugin : IServiceProviderPlugin
    {
        private readonly IHttpClientRebuilder _httpClientRebuilder;

        public HttpClientFactoryPlugin(IHttpClientRebuilder httpClientRebuilder)
        {
            _httpClientRebuilder = httpClientRebuilder;
        }

        public Type Type => typeof(IHttpClientFactory);
        public bool ContinueDecoration => false;

        public object GetService(object originalObject)
        {
            return new HttpClientFactoryWrapper(originalObject as IHttpClientFactory, _httpClientRebuilder);
        }
    }
}