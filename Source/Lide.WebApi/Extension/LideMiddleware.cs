using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Lide.Core.Model.Settings;
using Lide.WebApi.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lide.WebApi.Extension
{
    public class LideMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private string _propagateSettings;
        private string _propagateContent;
        private bool _enabled;
        private int _depth;

        public LideMiddleware(RequestDelegate next, IOptions<AppSettings> settings)
        {
            _next = next;
            _appSettings = settings?.Value ?? new AppSettings();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await CheckAndParse(httpContext).ConfigureAwait(false);
            if (_enabled)
            {
                if (_depth == 0)
                {
                    var settings = TryParseBody<PropagateSettings>(_propagateSettings);
                    var content = TryParseBody<Dictionary<string, byte[]>>(_propagateContent);
                    SetupLide(httpContext, settings, content);
                }
                else
                {
                    var originalContentField = httpContext.Request.Form.Files.First(x => x.Name == PropagateProperties.OriginalContent);
                    var originalContentStream = originalContentField.OpenReadStream();
                    httpContext.Request.Body = originalContentStream;
                    httpContext.Request.Headers.Clear();
                    foreach (var (key, value) in originalContentField.Headers)
                    {
                        httpContext.Request.Headers.Add(key, value);
                    }
                }
            }

            await _next(httpContext).ConfigureAwait(false);
            if (_enabled)
            {

            }
        }

        private void SetupLide(HttpContext httpContext, PropagateSettings settings, )
        {
            var scope = httpContext.RequestServices.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var wrapper = new ServiceProviderWrapper(scopedProvider, scope.Dispose);
            httpContext.RequestServices = wrapper;
            wrapper.SettingsProvider.Initialize(_appSettings, settings, _depth);
            wrapper.ScopeIdProvider.SetRootScopeId(wrapper.SettingsProvider.PropagateSettings.RootScopeId);
            wrapper.SettingsProvider.PropagateSettings.RootScopeId ??= wrapper.ScopeIdProvider.GetRootScopeId();
            wrapper.SettingsProvider.PropagateSettings.RequestDepthLevel++;
        }

        private async Task CheckAndParse(HttpContext httpContext)
        {
            _depth = Convert.ToInt32(httpContext.Request.Headers[PropagateProperties.Enabled].FirstOrDefault() ?? "0");
            _enabled = Convert.ToBoolean(httpContext.Request.Headers[PropagateProperties.Enabled].FirstOrDefault() ?? "false");
            _enabled = _enabled || Convert.ToBoolean(httpContext.Request.Query[PropagateProperties.Enabled].FirstOrDefault() ?? "false");
            _propagateSettings = httpContext.Request.Query[PropagateProperties.PropagateSettings].FirstOrDefault();
            _propagateContent = httpContext.Request.Query[PropagateProperties.PropagateContent].FirstOrDefault();

            var case1 = _enabled && string.IsNullOrEmpty(_propagateSettings) && string.IsNullOrEmpty(_propagateContent);
            var case2 = !_enabled && _appSettings.SearchHttpBody;
            if (case1 || case2)
            {
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
                var strRequestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                var jsonBody = TryParseBody<Dictionary<string, object>>(strRequestBody);

                _enabled = _enabled || Convert.ToBoolean(jsonBody[PropagateProperties.Enabled] ?? "false");
                _propagateSettings = (string)jsonBody[PropagateProperties.PropagateSettings];
                _propagateContent = (string)jsonBody[PropagateProperties.PropagateContent];
            }
        }

        private static TTarget TryParseBody<TTarget>(string body)
            where TTarget : new()
        {
            try
            {
                return JsonSerializer.Deserialize<TTarget>(body);
            }
            catch
            {
                try
                {
                    var fromBase64 = Convert.FromBase64String(body);
                    var originalString = Encoding.UTF8.GetString(fromBase64);
                    return JsonSerializer.Deserialize<TTarget>(originalString);
                }
                catch
                {
                    return new TTarget();
                }
            }
        }
    }
}