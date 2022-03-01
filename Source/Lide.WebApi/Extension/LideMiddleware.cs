using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "new MemoryStream.ConfigureAwait none-sense")]
    public class LideMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public LideMiddleware(RequestDelegate next, IOptions<AppSettings> settings)
        {
            _next = next;
            _appSettings = settings?.Value ?? new AppSettings();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var depth = Convert.ToInt32(httpContext.Request.Headers[PropagateProperties.Enabled].FirstOrDefault() ?? "0");
            var enabled = depth > 0
                          || Convert.ToBoolean(httpContext.Request.Headers[PropagateProperties.Enabled].FirstOrDefault() ?? "false")
                          || Convert.ToBoolean(httpContext.Request.Query[PropagateProperties.Enabled].FirstOrDefault() ?? "false")
                          || await GetEnabledFromBody(httpContext).ConfigureAwait(false);

            if (enabled)
            {
                await ExecuteLide(httpContext).ConfigureAwait(false);
                return;
            }

            await _next(httpContext).ConfigureAwait(false);
        }

        private async Task ExecuteLide(HttpContext httpContext)
        {
            var scope = httpContext.RequestServices.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var wrapper = new ServiceProviderWrapper(scopedProvider, scope.Dispose);

            await ParseAndReplaceRequestBody(httpContext, wrapper);

            await using var memoryResponseBody = new MemoryStream();
            var originalResponseBody = httpContext.Response.Body;
            httpContext.Response.Body = memoryResponseBody;

            void Handler(ConcurrentDictionary<string, byte[]> container, string requestPath)
            {
                container.TryAdd(PropagateProperties.PropagateSettings, wrapper.PropagateContentHandler.ParentData[PropagateProperties.PropagateSettings]);
            }

            try
            {
                wrapper.PropagateContentHandler.PrepareForChild += Handler;
                await _next(httpContext).ConfigureAwait(false);
            }
            finally
            {
                wrapper.PropagateContentHandler.PrepareForChild -= Handler;
            }

            var dataForParent = wrapper.PropagateContentHandler.GetDataForParent();
            dataForParent[PropagateProperties.OriginalResponse] = memoryResponseBody.ToArray();
            var serializedResponse = wrapper.BinarySerializeProvider.Serialize(dataForParent);
            var compressedResponse = wrapper.CompressionProvider.Compress(serializedResponse);

            PrepareResponseAsAFile(httpContext);

            httpContext.Response.Body = originalResponseBody;
            httpContext.Response.ContentLength = compressedResponse.Length;
            await httpContext.Response.Body.WriteAsync(compressedResponse).ConfigureAwait(false);
        }

        private static void PrepareResponseAsAFile(HttpContext httpContext)
        {
            var depth = Convert.ToInt32(httpContext.Request.Headers[PropagateProperties.Enabled].FirstOrDefault() ?? "0");
            if (depth != 0)
            {
                return;
            }

            var path = httpContext.Request.Path.Value?.Replace('/', '.')?[1..];
            var action = httpContext.Request.RouteValues.ContainsKey("controller")
                ? httpContext.Request.RouteValues["controller"] + "." + httpContext.Request.RouteValues["action"]
                : "unknown";
            var filename = $"{path ?? action}.lide";
            httpContext.Response.ContentType = "text/binary";
            httpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}; filename*=UTF-8''{filename}");
        }

        private async Task ParseAndReplaceRequestBody(HttpContext httpContext, ServiceProviderWrapper wrapper)
        {
            await using var currentRequestBodyCopy = new MemoryStream();
            await httpContext.Request.Body.CopyToAsync(currentRequestBodyCopy).ConfigureAwait(false);

            var depth = Convert.ToInt32(httpContext.Request.Headers[PropagateProperties.Enabled].FirstOrDefault() ?? "0");
            if (depth == 0)
            {
                using var reader = new StreamReader(currentRequestBodyCopy, Encoding.UTF8, leaveOpen: true);
                var strRequestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
                var maybeJsonBody = TryParseFromJsonOrBase64<Dictionary<string, object>>(strRequestBody);
                var settingsData = httpContext.Request.Headers[PropagateProperties.PropagateSettings].FirstOrDefault() ??
                               httpContext.Request.Query[PropagateProperties.PropagateSettings].FirstOrDefault() ??
                               Convert.ToString(maybeJsonBody[PropagateProperties.PropagateSettings]);
                var settings = TryParseFromJsonOrBase64<PropagateSettings>(settingsData);

                var headers = httpContext.Request.Headers.ToDictionary(
                    x => x.Key,
                    x => x.Value.ToArray());

                wrapper.PropagateContentHandler.ParentData[PropagateProperties.OriginalRequest] = currentRequestBodyCopy.ToArray();
                wrapper.PropagateContentHandler.ParentData[PropagateProperties.OriginalHeaders] = wrapper.BinarySerializeProvider.Serialize(headers);
                wrapper.PropagateContentHandler.ParentData[PropagateProperties.PropagateSettings] = wrapper.BinarySerializeProvider.Serialize(settings);
                wrapper.SettingsProvider.Initialize(_appSettings, settings);
                wrapper.SettingsProvider.OriginRequestPath = httpContext.Request.Path;
            }

            if (depth > 0)
            {
                var decompressedRequestBody = wrapper.CompressionProvider.Decompress(currentRequestBodyCopy.ToArray());
                var deserializedRequestBody = wrapper.BinarySerializeProvider.Deserialize<Dictionary<string, byte[]>>(decompressedRequestBody);
                wrapper.PropagateContentHandler.ParentData = deserializedRequestBody;
                var originalContentData = wrapper.PropagateContentHandler.ParentData[PropagateProperties.OriginalRequest];
                var originalHeadersData = wrapper.PropagateContentHandler.ParentData[PropagateProperties.OriginalHeaders];
                var propagateSettingsData = wrapper.PropagateContentHandler.ParentData[PropagateProperties.PropagateSettings];

                var settings = wrapper.BinarySerializeProvider.Deserialize<PropagateSettings>(propagateSettingsData);
                var originalHeadersDictionary = wrapper.BinarySerializeProvider.Deserialize<Dictionary<string, string[]>>(originalHeadersData);
                var originalContentReplaceStream = new MemoryStream(originalContentData);
                foreach (var (key, values) in originalHeadersDictionary)
                {
                    httpContext.Request.Headers.Add(key, values);
                }

                await httpContext.Request.Body.DisposeAsync().ConfigureAwait(false);
                httpContext.Request.Body = originalContentReplaceStream;
                wrapper.SettingsProvider.Initialize(_appSettings, settings);
                wrapper.SettingsProvider.OriginRequestPath = httpContext.Request.Path;
            }
        }

        private async Task<bool> GetEnabledFromBody(HttpContext httpContext)
        {
            if (!_appSettings.SearchHttpBody)
            {
                return false;
            }

            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
            var strRequestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            var jsonBody = TryParseFromJsonOrBase64<Dictionary<string, object>>(strRequestBody);
            return Convert.ToBoolean(jsonBody[PropagateProperties.Enabled] ?? "false");
        }

        private static TTarget TryParseFromJsonOrBase64<TTarget>(string data)
            where TTarget : new()
        {
            try
            {
                return JsonSerializer.Deserialize<TTarget>(data);
            }
            catch
            {
                try
                {
                    var fromBase64 = Convert.FromBase64String(data);
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