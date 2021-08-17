using System.Net.Http;

namespace Lide.WebAPI
{
    public class HttpClientFactoryWrapper : IHttpClientFactory
    {
        private readonly IHttpClientFactory _originalObject;
        private readonly IHttpHeaderProcessor _httpHeaderProcessor;

        public HttpClientFactoryWrapper(IHttpClientFactory originalObject, IHttpHeaderProcessor httpHeaderProcessor)
        {
            _originalObject = originalObject;
            _httpHeaderProcessor = httpHeaderProcessor;
        }
        
        public HttpClient CreateClient(string name)
        {
            var client = _originalObject.CreateClient(name);
            _httpHeaderProcessor.AddHeaders(client);
            return client;
        }
    }
}