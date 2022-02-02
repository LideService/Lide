using System;
using System.Net.Http;
using Lide.Core.Contract.Plugin;
using Lide.WebApi.Contract;

namespace Lide.WebApi.Plugin
{
    public class HttpClientPlugin : IServiceProviderPlugin
    {
        private readonly IHttpClientRebuilder _httpClientRebuilder;

        public HttpClientPlugin(IHttpClientRebuilder httpClientRebuilder)
        {
            _httpClientRebuilder = httpClientRebuilder;
        }

        public Type Type => typeof(HttpClient);
        public bool ContinueDecoration => false;

        public object GetService(object originalObject)
        {
            var originalClient = originalObject as HttpClient;
            var newClient = _httpClientRebuilder.RebuildNewClient(originalClient);
            return newClient;
        }
    }
}