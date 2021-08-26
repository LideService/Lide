using System;
using System.Net.Http;
using Lide.Core.Contract.Plugin;
using Lide.WebApi.Contract;
using Microsoft.AspNetCore.Http.Features;

namespace Lide.WebApi.Plugin
{
    public class HttpClientPlugin : IServiceProviderPlugin
    {
        private readonly IHttpHeaderProcessor _httpHeaderProcessor;

        public HttpClientPlugin(IHttpHeaderProcessor httpHeaderProcessor)
        {
            _httpHeaderProcessor = httpHeaderProcessor;
        }

        public Type Type => typeof(HttpClient);
        public bool ContinueDecoration => false;

        public object GetService(object originalObject)
        {
            var client = originalObject as HttpClient;
            _httpHeaderProcessor.AddHeaders(client);
            return client;
        }
    }
}