using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Lide.Core;
using Lide.Core.Contract.Provider;
using Lide.Core.Facade;
using Lide.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Lide.WebApi.Extension
{
    public class ContainerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICompressionProvider _compressionProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IScopeProvider _scopeProvider;
        private readonly LideAppSettings _appSettings;

        public ContainerMiddleware(
            RequestDelegate next,
            IOptions<LideAppSettings> settings,
            ICompressionProvider compressionProvider,
            ISettingsProvider settingsProvider,
            IScopeProvider scopeProvider)
        {
            _next = next;
            _compressionProvider = compressionProvider;
            _settingsProvider = settingsProvider;
            _scopeProvider = scopeProvider;
            _appSettings = settings?.Value ?? new LideAppSettings();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var headers = httpContext.Request.Headers;
            var query = httpContext.Request.Query;
            var lideEnabledHeader = headers.ContainsKey(LideProperties.LideEnabled) && Convert.ToBoolean(headers[LideProperties.LideEnabled].FirstOrDefault() ?? "false");
            var lideEnabledQuery = query.ContainsKey(LideProperties.LideEnabled) && Convert.ToBoolean(query[LideProperties.LideEnabled].FirstOrDefault() ?? "false");

            lideEnabledHeader = true;
            if (lideEnabledHeader)
            {
                var compressionUsed = headers.ContainsKey(LideProperties.LideCompression) ? headers[LideProperties.LideCompression].FirstOrDefault() : null;
                var previousScopeId = headers.ContainsKey(LideProperties.LideScopeId) ? headers[LideProperties.LideScopeId].FirstOrDefault() : null;
                var settings = headers.ContainsKey(LideProperties.LideSettings) ? headers[LideProperties.LideSettings].FirstOrDefault() : null;
                SetupLide(httpContext, compressionUsed, previousScopeId, settings);
            }
            else
            if (lideEnabledQuery)
            {
                var compressionUsed = query.ContainsKey(LideProperties.LideCompression) ? query[LideProperties.LideCompression].FirstOrDefault() : null;
                var previousScopeId = query.ContainsKey(LideProperties.LideScopeId) ? query[LideProperties.LideScopeId].FirstOrDefault() : null;
                var settings = query.ContainsKey(LideProperties.LideSettings) ? query[LideProperties.LideSettings].FirstOrDefault() : null;
                SetupLide(httpContext, compressionUsed, previousScopeId, settings);
            }
            else
            if (_appSettings.SearchHttpBodyOrQuery)
            {
                await SearchBodyForSettings(httpContext).ConfigureAwait(false);
            }

            await _next(httpContext).ConfigureAwait(false);
        }

        private async Task SearchBodyForSettings(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
            var strRequestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            if (string.IsNullOrEmpty(strRequestBody))
            {
                return;
            }

            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(strRequestBody) ?? new Dictionary<string, object>();
            var lideEnabled = jsonObject.ContainsKey(LideProperties.LideEnabled) && Convert.ToBoolean(((JsonElement)jsonObject[LideProperties.LideEnabled]).GetString());
            if (lideEnabled)
            {
                var compressionUsed = jsonObject.ContainsKey(LideProperties.LideCompression) ? ((JsonElement)jsonObject[LideProperties.LideCompression]).GetString() : null;
                var previousScopeId = jsonObject.ContainsKey(LideProperties.LideScopeId) ? ((JsonElement)jsonObject[LideProperties.LideScopeId]).GetString() : null;
                var settings = jsonObject.ContainsKey(LideProperties.LideSettings) ? ((JsonElement)jsonObject[LideProperties.LideSettings]).GetString() : null;
                SetupLide(httpContext, compressionUsed, previousScopeId, settings);
            }
        }

        private void SetupLide(HttpContext httpContext, string compressionUsed, string previousScopeId, string settings)
        {
            var compressed = Convert.ToBoolean(compressionUsed);
            var propagateSettings = new LidePropagateSettings(compressed ? _compressionProvider.DecompressString(settings) : settings);
            _settingsProvider.LideAppSettings = _appSettings;
            _settingsProvider.LidePropagateSettings = propagateSettings;
            _scopeProvider.SetPreviousScopes(previousScopeId);
            httpContext.RequestServices = new ServiceProviderWrapper(httpContext.RequestServices);
        }
    }
}