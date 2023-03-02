using System.IO;
using System.IO.Compression;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider;

public class CompressionProvider : ICompressionProvider
{
    public byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using var stream = new DeflateStream(output, CompressionLevel.Optimal);
        stream.Write(data, 0, data.Length);
        stream.Flush();
        return output.ToArray();
    }

    public byte[] Decompress(byte[] compressed)
    {
        using var input = new MemoryStream(compressed);
        using var output = new MemoryStream();
        using var stream = new DeflateStream(input, CompressionMode.Decompress);
        stream.CopyTo(output);
        return output.ToArray();
    }
}