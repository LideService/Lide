using System;
using System.Collections.Concurrent;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class PropagateContentHandler : IPropagateContentHandler
    {
        public event PrepareOutgoingRequestHandler PrepareOutgoingRequest;
        public event ParseOutgoingResponseHandler ParseOutgoingResponse;
        public event PrepareOwnResponseHandler PrepareOwnResponse;
        public event ParseOwnRequestHandler ParseOwnRequest;

        public void ParseDataFromOwnRequest(ConcurrentDictionary<string, byte[]> content)
        {
            ParseOwnRequest?.Invoke(content);
        }

        public void PrepareDataForOutgoingRequest(ConcurrentDictionary<string, byte[]> container, string path, long requestId, byte[] content)
        {
            PrepareOutgoingRequest?.Invoke(container, path, requestId, content);
        }

        public void ParseDataFromOutgoingResponse(ConcurrentDictionary<string, byte[]> content, string path, long requestId, Exception exception)
        {
            ParseOutgoingResponse?.Invoke(content, path, requestId, exception);
        }

        public void PrepareDataForOwnResponse(ConcurrentDictionary<string, byte[]> container)
        {
            PrepareOwnResponse?.Invoke(container);
        }
    }
}