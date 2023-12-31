using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lide.Core.Contract.Provider;
using Lide.Core.Model.Settings;
using Lide.Decorators;

namespace Lide.WebAPI.Wrappers;

public class HttpMessageHandlerWrapper : HttpMessageHandler
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IPropagateContentHandler _propagateContentHandler;
    private readonly IBinarySerializeProvider _binarySerializeProvider;
    private readonly ICompressionProvider _compressionProvider;
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsyncInvoke;
    private long _requestId;

    public HttpMessageHandlerWrapper(
        HttpMessageHandler originalObject,
        ISettingsProvider settingsProvider,
        IPropagateContentHandler propagateContentHandler,
        IBinarySerializeProvider binarySerializeProvider,
        ICompressionProvider compressionProvider)
    {
        _settingsProvider = settingsProvider;
        _propagateContentHandler = propagateContentHandler;
        _binarySerializeProvider = binarySerializeProvider;
        _compressionProvider = compressionProvider;
        var sendAsyncMethodInfo = typeof(HttpMessageHandler)
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .First(x => x.Name == "SendAsync");

        _sendAsyncInvoke = (request, cancellationToken) =>
        {
            try
            {
                return (Task<HttpResponseMessage>)sendAsyncMethodInfo.Invoke(originalObject, new object[] { request, cancellationToken });
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
                throw;
            }
        };
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).Result;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_settingsProvider.IsAddressAllowed(request.RequestUri?.ToString() ?? string.Empty))
        {
            return await _sendAsyncInvoke(request, cancellationToken).ConfigureAwait(false);
        }

        var requestId = Interlocked.Increment(ref _requestId);
        var requestContainer = new ConcurrentDictionary<string, byte[]>();
        requestContainer[PropagateProperties.OriginalQuery] = Encoding.UTF8.GetBytes(request.RequestUri?.Query ?? string.Empty);
        requestContainer[PropagateProperties.OriginalContent] = Array.Empty<byte>();
        var requestContent = (byte[])null;
        if (request.Content != null)
        {
            requestContent = await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            requestContainer[PropagateProperties.OriginalContent] = requestContent;
            request.Content.Dispose();
        }

        _propagateContentHandler.PrepareDataForOutgoingRequest(requestContainer, request.RequestUri?.AbsolutePath, requestId, requestContent);
        var propagateData = new Dictionary<string, byte[]>(requestContainer);
        var serialized = _binarySerializeProvider.Serialize(propagateData);
        var compressed = _compressionProvider.Compress(serialized);
        var propagateContent = new ByteArrayContent(compressed);
        request.Headers.Add(PropagateProperties.Enabled, "true");
        request.Headers.Add(PropagateProperties.Depth, _settingsProvider.NextDepth.ToString());
        request.Content = propagateContent;

        try
        {
            var response = await _sendAsyncInvoke(request, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            var decompressed = _compressionProvider.Decompress(content);
            var deserialized = _binarySerializeProvider.Deserialize<Dictionary<string, byte[]>>(decompressed);
            var responseContainer = new ConcurrentDictionary<string, byte[]>(deserialized);
            _propagateContentHandler.ParseDataFromOutgoingResponse(responseContainer, request.RequestUri?.AbsolutePath, requestId, null);
            response.Content.Dispose();
            response.Content = new ByteArrayContent(responseContainer[PropagateProperties.OriginalContent]);

            return response;
        }
        catch (Exception e)
        {
            _propagateContentHandler.ParseDataFromOutgoingResponse(null, request.RequestUri?.AbsolutePath, requestId, e);
            ExceptionDispatchInfo.Capture(e).Throw();
            throw;
        }
    }
}