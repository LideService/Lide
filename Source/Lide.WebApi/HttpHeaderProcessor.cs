using System.Net.Http;
using Lide.Core.Contract.Provider;
using Lide.Core.Model.Settings;
using Lide.WebApi.Contract;

namespace Lide.WebApi
{
    public class HttpHeaderProcessor : IHttpHeaderProcessor
    {
        private readonly ICompressionProvider _compressionProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IScopeIdProvider _scopeIdProvider;

        public HttpHeaderProcessor(
            ICompressionProvider compressionProvider,
            ISettingsProvider settingsProvider,
            IScopeIdProvider scopeIdProvider)
        {
            _compressionProvider = compressionProvider;
            _settingsProvider = settingsProvider;
            _scopeIdProvider = scopeIdProvider;
        }

        public void AddHeaders(HttpClient httpClient)
        {
            var compressedSettings = _compressionProvider.CompressString(_settingsProvider.PropagateSettingsString);
            var scopeId = _scopeIdProvider.GetScopeId();
            httpClient.DefaultRequestHeaders.Add(PropagateProperties.Enabled, "true");
            httpClient.DefaultRequestHeaders.Add(PropagateProperties.Compression, "true");
            httpClient.DefaultRequestHeaders.Add(PropagateProperties.ScopeId, scopeId);
            httpClient.DefaultRequestHeaders.Add(PropagateProperties.Settings, compressedSettings);
        }
    }
}