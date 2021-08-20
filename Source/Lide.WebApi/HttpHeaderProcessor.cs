using System.Net.Http;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;
using Lide.WebApi.Contract;

namespace Lide.WebApi
{
    public class HttpHeaderProcessor : IHttpHeaderProcessor
    {
        private readonly ICompressionProvider _compressionProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IScopeProvider _scopeProvider;

        public HttpHeaderProcessor(
            ICompressionProvider compressionProvider,
            ISettingsProvider settingsProvider,
            IScopeProvider scopeProvider)
        {
            _compressionProvider = compressionProvider;
            _settingsProvider = settingsProvider;
            _scopeProvider = scopeProvider;
        }

        public void AddHeaders(HttpClient httpClient)
        {
            var settings = _settingsProvider.LidePropagateSettings.ToString();
            var compressedSettings = _compressionProvider.CompressString(settings);
            var scopeId = _scopeProvider.GetScopeId();
            httpClient.DefaultRequestHeaders.Add(LideProperties.LideEnabled, "true");
            httpClient.DefaultRequestHeaders.Add(LideProperties.LideCompression, "true");
            httpClient.DefaultRequestHeaders.Add(LideProperties.LideScopeId, scopeId);
            httpClient.DefaultRequestHeaders.Add(LideProperties.LideSettings, compressedSettings);
        }
    }
}