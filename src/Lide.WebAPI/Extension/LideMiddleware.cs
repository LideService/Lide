using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Lide.Core.Model;
using Lide.Core.Model.Settings;
using Lide.WebAPI.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lide.WebAPI.Extension;

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

    [DebuggerStepThrough]
    [DebuggerHidden]
    public async Task Invoke(HttpContext httpContext)
    {
        var enabled =
            Convert.ToBoolean(httpContext.Request.Headers[PropagateProperties.Enabled].FirstOrDefault() ?? "false")
            || Convert.ToBoolean(httpContext.Request.Query[PropagateProperties.Enabled].FirstOrDefault() ?? "false");

        if (enabled)
        {
            await ExecuteLide(httpContext).ConfigureAwait(false);
            return;
        }

        await _next(httpContext).ConfigureAwait(false);
    }

    [DebuggerStepThrough]
    [DebuggerHidden]
    private async Task ExecuteLide(HttpContext httpContext)
    {
        var originalServices = httpContext.RequestServices;
        using var scope = httpContext.RequestServices.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        await using var wrapper = new ServiceProviderWrapper(scopedProvider);
        httpContext.RequestServices = wrapper;

        await ParseAndReplaceRequestBody(httpContext, wrapper);

        await using var memoryResponseBody = new MemoryStream();
        var originalResponseBody = httpContext.Response.Body;
        httpContext.Response.Body = memoryResponseBody;

        try
        {
            await _next(httpContext).ConfigureAwait(false);
        }
        finally
        {
            httpContext.RequestServices = originalServices;
        }

        var container = new ConcurrentDictionary<string, byte[]>();
        container[PropagateProperties.OriginalContent] = memoryResponseBody.ToArray();
        wrapper.PropagateContentHandler.PrepareDataForOwnResponse(container);
        var dataForParent = new Dictionary<string, byte[]>(container);
        var serializedData = wrapper.BinarySerializeProvider.Serialize(dataForParent);
        var compressedData = wrapper.CompressionProvider.Compress(serializedData);
        var response = compressedData;

        var depth = Convert.ToInt32(httpContext.Request.Headers[PropagateProperties.Depth].FirstOrDefault() ?? "0");
        if (depth == 0)
        {
            var lideResponse = new LideResponse()
            {
                Path = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}",
                ContentData = compressedData,
            };
            var compressedLideResponse = wrapper.BinarySerializeProvider.Serialize(lideResponse);
            response = wrapper.CompressionProvider.Compress(compressedLideResponse);
        }

        PrepareResponseAsAFile(httpContext);

        httpContext.Response.Body = originalResponseBody;
        httpContext.Response.ContentLength = response.Length;
        await httpContext.Response.Body.WriteAsync(response).ConfigureAwait(false);
    }

    private static void PrepareResponseAsAFile(HttpContext httpContext)
    {
        var depth = Convert.ToInt32(httpContext.Request.Headers[PropagateProperties.Depth].FirstOrDefault() ?? "0");
        if (depth != 0)
        {
            return;
        }

        var path = (httpContext.Request.Path.Value?.Replace('/', '.'))?[1..];
        var action = httpContext.Request.RouteValues.ContainsKey("controller")
            ? httpContext.Request.RouteValues["controller"] + "." + httpContext.Request.RouteValues["action"]
            : "unknown";
        var filename = $"{path ?? action}.lide";
        httpContext.Response.ContentType = "blob";
        httpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}; filename*=UTF-8''{filename}");
    }

    private async Task ParseAndReplaceRequestBody(HttpContext httpContext, ServiceProviderWrapper wrapper)
    {
        httpContext.Request.EnableBuffering();
        httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
        await using var currentRequestBodyCopy = new MemoryStream();
        await httpContext.Request.Body.CopyToAsync(currentRequestBodyCopy).ConfigureAwait(false);
        var originalBodyContent = currentRequestBodyCopy.ToArray();

        var depth = Convert.ToInt32(httpContext.Request.Headers[PropagateProperties.Depth].FirstOrDefault() ?? "0");
        if (depth == 0)
        {
            var settingsData =
                httpContext.Request.Headers[PropagateProperties.PropagateSettings].FirstOrDefault() ??
                httpContext.Request.Query[PropagateProperties.PropagateSettings].FirstOrDefault();
            var settings = TryParseFromJsonOrBase64<PropagateSettings>(settingsData);

            var container = new ConcurrentDictionary<string, byte[]>();
            container[PropagateProperties.OriginalContent] = originalBodyContent;
            container[PropagateProperties.OriginalQuery] = Encoding.UTF8.GetBytes(httpContext.Request.QueryString.Value ?? string.Empty);
            container[PropagateProperties.PropagateSettings] = wrapper.BinarySerializeProvider.Serialize(settings);
            wrapper.SettingsProvider.Initialize(_appSettings, settings, httpContext.Request.Path);
            wrapper.PropagateContentHandler.ParseDataFromOwnRequest(container);
        }

        if (depth > 0)
        {
            var decompressedRequestBody = wrapper.CompressionProvider.Decompress(originalBodyContent);
            var deserializedRequestBody = wrapper.BinarySerializeProvider.Deserialize<Dictionary<string, byte[]>>(decompressedRequestBody);
            var container = new ConcurrentDictionary<string, byte[]>(deserializedRequestBody);

            var settingsFormHeader =
                httpContext.Request.Headers[PropagateProperties.PropagateSettings].FirstOrDefault() ??
                httpContext.Request.Query[PropagateProperties.PropagateSettings].FirstOrDefault();
            var settings = TryParseFromJsonOrBase64<PropagateSettings>(settingsFormHeader);

            if (container.TryGetValue(PropagateProperties.PropagateSettings, out var value))
            {
                var propagateSettingsData = value;
                settings = wrapper.BinarySerializeProvider.Deserialize<PropagateSettings>(propagateSettingsData);
            }

            wrapper.SettingsProvider.Initialize(_appSettings, settings, httpContext.Request.Path);
            wrapper.PropagateContentHandler.ParseDataFromOwnRequest(container);

            await httpContext.Request.Body.DisposeAsync().ConfigureAwait(false);
            var originalContentData = container[PropagateProperties.OriginalContent];
            var queryString = Encoding.UTF8.GetString(container[PropagateProperties.OriginalQuery]);
            var originalContentReplaceStream = new MemoryStream(originalContentData);
            httpContext.Request.Body = originalContentReplaceStream;
            httpContext.Request.QueryString = new QueryString(queryString);
        }
    }

    private static TTarget TryParseFromJsonOrBase64<TTarget>(string data)
        where TTarget : new()
    {
        if (data == null)
        {
            return new TTarget();
        }

        try
        {
            return JsonSerializer.Deserialize<TTarget>(data) ?? new TTarget();
        }
        catch
        {
            try
            {
                var fromBase64 = Convert.FromBase64String(data);
                var originalString = Encoding.UTF8.GetString(fromBase64);
                return JsonSerializer.Deserialize<TTarget>(originalString) ?? new TTarget();
            }
            catch
            {
                return new TTarget();
            }
        }
    }
}