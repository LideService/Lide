using System.Net.Http;
using Lide.Core.Contract.Provider;
using Lide.WebAPI.Contract;

namespace Lide.WebAPI
{
    public class HttpHeaderProcessor : IHttpHeaderProcessor
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IScopeProvider _scopeProvider;

        public HttpHeaderProcessor(
            ISettingsProvider settingsProvider,
            IScopeProvider scopeProvider)
        {
            _settingsProvider = settingsProvider;
            _scopeProvider = scopeProvider;
        }
        
        public void AddHeaders(HttpClient httpClient)
        {
            // TODO
            httpClient.DefaultRequestHeaders.Add("Lide.Compression","true");
            httpClient.DefaultRequestHeaders.Add("Lide.Enable","");
            httpClient.DefaultRequestHeaders.Add("Lide.Settings","");
        }
    }
}