using System;

namespace Lide.Core.Model;

public class SubstituteOwnContent
{
    public int TestId { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public byte[] Query { get; set; } = Array.Empty<byte>();
}