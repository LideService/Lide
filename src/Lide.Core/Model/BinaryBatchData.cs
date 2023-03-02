using System;

namespace Lide.Core.Model;

public class BinaryBatchData
{
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public int EndPosition { get; set; }
}