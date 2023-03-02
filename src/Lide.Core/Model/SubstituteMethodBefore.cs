using System;

namespace Lide.Core.Model;

public class SubstituteMethodBefore
{
    public long CallId { get; set; }
    public string MethodSignature { get; set; } = string.Empty;
    public byte[] InputParameters { get; set; } = Array.Empty<byte>();
    public byte[] SerializedObject { get; set; } = Array.Empty<byte>();
}