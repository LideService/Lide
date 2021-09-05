using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Lide.Core;
using Lide.Core.Model.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lide.WebApi.Extension
{
    public class ContainerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public ContainerMiddleware(RequestDelegate next, IOptions<AppSettings> settings)
        {
            _next = next;
            _appSettings = settings?.Value ?? new AppSettings();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // TODO: somehow allow only executing assembly namespace or so.
            var headers = httpContext.Request.Headers;
            var query = httpContext.Request.Query;
            var lideEnabledHeader = headers.ContainsKey(PropagateProperties.Enabled) && Convert.ToBoolean(headers[PropagateProperties.Enabled].FirstOrDefault() ?? "false");
            var lideEnabledQuery = query.ContainsKey(PropagateProperties.Enabled) && Convert.ToBoolean(query[PropagateProperties.Enabled].FirstOrDefault() ?? "false");

            if (lideEnabledHeader)
            {
                var rootScopeId = headers.ContainsKey(PropagateProperties.RootScopeId) ? headers[PropagateProperties.RootScopeId].FirstOrDefault() : null;
                var settings = headers.ContainsKey(PropagateProperties.Settings) ? headers[PropagateProperties.Settings].FirstOrDefault() : null;
                SetupLide(httpContext, rootScopeId, settings);
            }
            else
            if (lideEnabledQuery)
            {
                var rootScopeId = query.ContainsKey(PropagateProperties.RootScopeId) ? query[PropagateProperties.RootScopeId].FirstOrDefault() : null;
                var settings = query.ContainsKey(PropagateProperties.Settings) ? query[PropagateProperties.Settings].FirstOrDefault() : null;
                SetupLide(httpContext, rootScopeId, settings);
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
                var rootScopeId = jsonObject.ContainsKey(PropagateProperties.RootScopeId) ? ((JsonElement)jsonObject[PropagateProperties.RootScopeId]).GetString() : null;
                var settings = jsonObject.ContainsKey(PropagateProperties.Settings) ? ((JsonElement)jsonObject[PropagateProperties.Settings]).GetString() : null;
                SetupLide(httpContext, rootScopeId, settings);
            }
        }

        private void SetupLide(HttpContext httpContext, string rootId, string settings)
        {
            var scope = httpContext.RequestServices.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var wrapper = new ServiceProviderWrapper(scopedProvider, scope.Dispose);
            httpContext.RequestServices = wrapper;
            wrapper.SettingsProvider.SetData(_appSettings, settings);
            wrapper.ScopeIdProvider.SetRootScopeId(rootId);
        }
    }
}