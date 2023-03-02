using System;

namespace Lide.Core.Model;

public class SubstituteOutgoingRequest
{
    public string Path { get; set; } = string.Empty;
    public long RequestId { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
}