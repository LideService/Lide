using System;

namespace Lide.Core.Model;

public class SubstituteMethodAfter
{
    public long CallId { get; set; }
    public bool IsException { get; set; }
    public byte[] OutputData { get; set; } = Array.Empty<byte>();
    public byte[] InputParameters { get; set; } = Array.Empty<byte>();
    public byte[] SerializedObject { get; set; } = Array.Empty<byte>();
}