using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Lide.Core.Contract.Provider
{
    public delegate void ParseOwnRequestHandler(ConcurrentDictionary<string, byte[]> content);
    public delegate void PrepareOutgoingRequestHandler(ConcurrentDictionary<string, byte[]> container, string path, long requestId, byte[] content);
    public delegate void ParseOutgoingResponseHandler(ConcurrentDictionary<string, byte[]> content, string path, long requestId, Exception exception);
    public delegate void PrepareOwnResponseHandler(ConcurrentDictionary<string, byte[]> container);
    public interface IPropagateContentHandler
    {
        event ParseOwnRequestHandler ParseOwnRequest;
        event PrepareOutgoingRequestHandler PrepareOutgoingRequest;
        event ParseOutgoingResponseHandler ParseOutgoingResponse;
        event PrepareOwnResponseHandler PrepareOwnResponse;

        void ParseDataFromOwnRequest(ConcurrentDictionary<string, byte[]> content);
        void PrepareDataForOutgoingRequest(ConcurrentDictionary<string, byte[]> container, string path, long requestId, byte[] content);
        void ParseDataFromOutgoingResponse(ConcurrentDictionary<string, byte[]> content, string path, long requestId, Exception exception);
        void PrepareDataForOwnResponse(ConcurrentDictionary<string, byte[]> container);
    }
}
