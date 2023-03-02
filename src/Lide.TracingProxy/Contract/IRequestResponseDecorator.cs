using System;
using System.Collections.Concurrent;

namespace Lide.TracingProxy.Contract;

public interface IRequestResponseDecorator : IObjectDecorator
{
    void ParseOutgoingResponse(ConcurrentDictionary<string, byte[]> content, string path, long requestId, Exception exception)
    {
    }

    void PrepareOwnResponse(ConcurrentDictionary<string, byte[]> container)
    {
    }

    void ParseOwnRequest(ConcurrentDictionary<string, byte[]> content)
    {
    }

    void PrepareOutgoingRequest(ConcurrentDictionary<string, byte[]> container, string path, long requestId, byte[] content)
    {
    }
}