using System;

namespace Lide.Core.Model;

public class LideResponse
{
    public byte[] ContentData { get; set; } = Array.Empty<byte>();
    public string Path { get; set; } = string.Empty;
}