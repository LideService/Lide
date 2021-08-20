using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Lide.Core.Contract.Provider;
using Lide.Core.Facade;
using Lide.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Lide.WebAPI.Extension
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
            IScopeProvider scopeProvider
            )
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
                await SearchBodyForSettings(httpContext);
            }
                            
            await _next(httpContext); 
        }
        
        private async Task SearchBodyForSettings(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            
            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
            var strRequestBody = await reader.ReadToEndAsync();
            httpContext.Request.Body.Seek(0,SeekOrigin.Begin);
            
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string,string>>(strRequestBody) ?? new Dictionary<string, string>();
            var lideEnabled = jsonObject.ContainsKey(LideProperties.LideEnabled) && Convert.ToBoolean(jsonObject[LideProperties.LideEnabled]);
            if (lideEnabled)
            {
                var compressionUsed = jsonObject.ContainsKey(LideProperties.LideCompression) ? jsonObject[LideProperties.LideCompression] : null;
                var previousScopeId = jsonObject.ContainsKey(LideProperties.LideScopeId) ? jsonObject[LideProperties.LideScopeId] : null;
                var settings = jsonObject.ContainsKey(LideProperties.LideSettings) ? jsonObject[LideProperties.LideSettings] : null;
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