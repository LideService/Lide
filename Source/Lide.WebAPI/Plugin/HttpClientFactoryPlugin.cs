using System;
using System.Net.Http;
using Lide.Core.Contract.Plugin;
using Lide.WebAPI.Contract;

namespace Lide.WebAPI.Plugin
{
    public class HttpClientFactoryPlugin : IServiceProviderPlugin
    {
        private readonly IHttpHeaderProcessor _httpHeaderProcessor;
        public Type Type => typeof(IHttpClientFactory);

        public HttpClientFactoryPlugin(IHttpHeaderProcessor httpHeaderProcessor)
        {
            _httpHeaderProcessor = httpHeaderProcessor;
        }
        
        public object GetService(object originalObject)
        {
            return new HttpClientFactoryWrapper(originalObject as IHttpClientFactory, _httpHeaderProcessor);
        }
    }
}