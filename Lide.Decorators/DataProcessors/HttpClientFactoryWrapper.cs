using System.Net.Http;

namespace Lide.Decorators.DataProcessors
{
    public class HttpClientFactoryWrapper : IHttpClientFactory
    {
        private readonly IHttpClientFactory _originalObject;

        public HttpClientFactoryWrapper(IHttpClientFactory originalObject)
        {
            _originalObject = originalObject;
        }
        
        public HttpClient CreateClient(string name)
        {
            var client = _originalObject.CreateClient(name);
            return client;
        }
    }
}