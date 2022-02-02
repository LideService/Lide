using System;
using System.Collections.Concurrent;
using System.Net.Http;
using Lide.WebApi.Contract;

namespace Lide.WebApi.Wrappers
{
    public class HttpClientFactoryWrapper : IHttpClientFactory, IDisposable
    {
        private readonly IHttpClientFactory _originalObject;
        private readonly IHttpClientRebuilder _httpClientRebuilder;
        private readonly ConcurrentDictionary<string, HttpClient> _clients;

        public HttpClientFactoryWrapper(IHttpClientFactory originalObject, IHttpClientRebuilder httpClientRebuilder)
        {
            _originalObject = originalObject;
            _httpClientRebuilder = httpClientRebuilder;
            _clients = new ConcurrentDictionary<string, HttpClient>();
        }

        public HttpClient CreateClient(string name)
        {
            if (_clients.ContainsKey(name))
            {
                return _clients[name];
            }

            var originalClient = _originalObject.CreateClient(name);
            var newClient = _httpClientRebuilder.RebuildNewClient(originalClient);
            var added = _clients.TryAdd(name, newClient);
            if (!added)
            {
                newClient.Dispose();
            }

            return _clients[name];
        }

        public void Dispose()
        {
            foreach (var client in _clients.Values)
            {
                client.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}