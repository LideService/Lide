using System;
using System.Collections.Concurrent;

namespace Lide.Decorators;

public interface IPropagateContentHandler
{
    void ParseDataFromOwnRequest(ConcurrentDictionary<string, byte[]> content);
    void PrepareDataForOutgoingRequest(ConcurrentDictionary<string, byte[]> container, string path, long requestId, byte[] content);
    void ParseDataFromOutgoingResponse(ConcurrentDictionary<string, byte[]> content, string path, long requestId, Exception exception);
    void PrepareDataForOwnResponse(ConcurrentDictionary<string, byte[]> container);
}