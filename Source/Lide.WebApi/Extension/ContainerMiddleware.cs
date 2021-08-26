using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Lide.Core;
using Lide.Core.Contract.Provider;
using Lide.Core.Model.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lide.WebApi.Extension
{
    public class ContainerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICompressionProvider _compressionProvider;
        private readonly AppSettings _appSettings;

        public ContainerMiddleware(
            RequestDelegate next,
            IOptions<AppSettings> settings,
            ICompressionProvider compressionProvider)
        {
            _next = next;
            _compressionProvider = compressionProvider;
            _appSettings = settings?.Value ?? new AppSettings();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var headers = httpContext.Request.Headers;
            var query = httpContext.Request.Query;
            var lideEnabledHeader = headers.ContainsKey(PropagateProperties.Enabled) && Convert.ToBoolean(headers[PropagateProperties.Enabled].FirstOrDefault() ?? "false");
            var lideEnabledQuery = query.ContainsKey(PropagateProperties.Enabled) && Convert.ToBoolean(query[PropagateProperties.Enabled].FirstOrDefault() ?? "false");

            if (lideEnabledHeader)
            {
                var compressionUsed = headers.ContainsKey(PropagateProperties.Compression) ? headers[PropagateProperties.Compression].FirstOrDefault() : null;
                var previousScopeId = headers.ContainsKey(PropagateProperties.ScopeId) ? headers[PropagateProperties.ScopeId].FirstOrDefault() : null;
                var settings = headers.ContainsKey(PropagateProperties.Settings) ? headers[PropagateProperties.Settings].FirstOrDefault() : null;
                SetupLide(httpContext, compressionUsed, previousScopeId, settings);
            }
            else
            if (lideEnabledQuery)
            {
                var compressionUsed = query.ContainsKey(PropagateProperties.Compression) ? query[PropagateProperties.Compression].FirstOrDefault() : null;
                var previousScopeId = query.ContainsKey(PropagateProperties.ScopeId) ? query[PropagateProperties.ScopeId].FirstOrDefault() : null;
                var settings = query.ContainsKey(PropagateProperties.Settings) ? query[PropagateProperties.Settings].FirstOrDefault() : null;
                SetupLide(httpContext, compressionUsed, previousScopeId, settings);
            }
            else
            if (_appSettings.SearchHttpBody)
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
            var lideEnabled = jsonObject.ContainsKey(PropagateProperties.Enabled) && ((JsonElement)jsonObject[PropagateProperties.Enabled]).GetBoolean();
            if (lideEnabled)
            {
                var compressionUsed = jsonObject.ContainsKey(PropagateProperties.Compression) ? ((JsonElement)jsonObject[PropagateProperties.Compression]).GetString() : null;
                var previousScopeId = jsonObject.ContainsKey(PropagateProperties.ScopeId) ? ((JsonElement)jsonObject[PropagateProperties.ScopeId]).GetString() : null;
                var settings = jsonObject.ContainsKey(PropagateProperties.Settings) ? ((JsonElement)jsonObject[PropagateProperties.Settings]).GetString() : null;
                SetupLide(httpContext, compressionUsed, previousScopeId, settings);
            }
        }

        private void SetupLide(HttpContext httpContext, string compressionUsed, string previousScopeId, string settings)
        {
            var scope = httpContext.RequestServices.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var wrapper = new ServiceProviderWrapper(scopedProvider, scope.Dispose);
            httpContext.RequestServices = wrapper;

            var compressed = Convert.ToBoolean(compressionUsed);
            var propagateSettings = compressed ? _compressionProvider.DecompressString(settings) : settings;
            wrapper.SettingsProvider.SetData(_appSettings, propagateSettings);
            wrapper.ScopeIdProvider.SetPreviousScopes(previousScopeId);
        }
    }
}