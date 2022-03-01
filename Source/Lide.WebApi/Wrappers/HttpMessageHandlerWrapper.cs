using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Lide.Core.Contract.Provider;
using Lide.Core.Model.Settings;

namespace Lide.WebApi.Wrappers
{
    public class HttpMessageHandlerWrapper : HttpMessageHandler
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IPropagateContentHandler _propagateContentHandler;
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly ICompressionProvider _compressionProvider;
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsyncInvoke;

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
            var sendAsyncMethodInfo = typeof(HttpMessageHandler).GetMethods().First(x => x.Name == "SendAsync");

            _sendAsyncInvoke = (request, cancellationToken) =>
            {
                try
                {
                    return sendAsyncMethodInfo.Invoke(originalObject, new object[] { request, cancellationToken }) as Task<HttpResponseMessage>;
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

            var propagateData = _propagateContentHandler.GetDataForChild(request.RequestUri?.AbsolutePath);
            if (request.Content != null)
            {
                var headers = request.Content.Headers.ToDictionary(
                    x => x.Key,
                    x => x.Value.ToArray());

                propagateData.Add(PropagateProperties.OriginalRequest, await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false));
                propagateData.Add(PropagateProperties.OriginalHeaders, _binarySerializeProvider.Serialize(headers));
                request.Content.Dispose();
            }

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
                _propagateContentHandler.ParseDataFromChild(deserialized, null, request.RequestUri?.AbsolutePath);
                response.Content.Dispose();
                response.Content = new ByteArrayContent(deserialized[PropagateProperties.OriginalResponse]);

                return response;
            }
            catch (Exception e)
            {
                _propagateContentHandler.ParseDataFromChild(null, e, request.RequestUri?.AbsolutePath);
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
        }
    }
}