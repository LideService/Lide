using System;
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
        private readonly IPropagateContentProvider _propagateContentProvider;
        private readonly IPropagateContentController _propagateContentController;
        private readonly IBinarySerializeProvider _binarySerializeProvider;
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsyncInvoke;

        public HttpMessageHandlerWrapper(
            HttpMessageHandler originalObject,
            ISettingsProvider settingsProvider,
            IPropagateContentProvider propagateContentProvider,
            IPropagateContentController propagateContentController,
            IBinarySerializeProvider binarySerializeProvider)
        {
            _settingsProvider = settingsProvider;
            _propagateContentProvider = propagateContentProvider;
            _propagateContentController = propagateContentController;
            _binarySerializeProvider = binarySerializeProvider;
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
            if (_settingsProvider.IsAddressAllowed(request.RequestUri?.OriginalString ?? string.Empty))
            {
                var multiPartContent = new MultipartFormDataContent();
                if (request.Content != null)
                {
                    var originalContent = new ByteArrayContent(await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false));
                    multiPartContent.Add(originalContent, PropagateProperties.OriginalContent, PropagateProperties.OriginalContent);
                    foreach (var (key, values) in request.Content.Headers)
                    {
                        originalContent.Headers.Add(key, values);
                    }
                }

                var propagateContent = new ByteArrayContent(await _propagateContentController.GetDataForRequest().ConfigureAwait(false));
                var settingsContent = new ByteArrayContent(_binarySerializeProvider.Serialize(_settingsProvider.PropagateSettings));
                multiPartContent.Add(propagateContent, PropagateProperties.PropagateContent, PropagateProperties.PropagateContent);
                multiPartContent.Add(settingsContent, PropagateProperties.PropagateSettings, PropagateProperties.PropagateSettings);
                request.Headers.Add(PropagateProperties.Enabled, _settingsProvider.PropagateHeaders.Enabled.ToString());
                request.Headers.Add(PropagateProperties.Depth, _settingsProvider.PropagateHeaders.NextDepth.ToString());
                request.Content = multiPartContent;
            }

            var response = await _sendAsyncInvoke(request, cancellationToken).ConfigureAwait(false);
            if (_settingsProvider.IsAddressAllowed(request.RequestUri?.OriginalString ?? string.Empty))
            {
                var content = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                await _propagateContentController.PutDataFromResponse(content).ConfigureAwait(false);
                var originalPayload = _propagateContentProvider.ReadDataFromResponse(PropagateProperties.OriginalContent);
                response.Content = new ByteArrayContent(originalPayload);
            }

            return response;
        }
    }
}