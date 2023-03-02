using System;

namespace Lide.Core.Model;

public class SubstituteOutgoingResponse
{
    public long RequestId { get; set; }
    public string Path { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public Exception Exception { get; set; }
}