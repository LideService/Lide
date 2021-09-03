using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class CompressionProvider : ICompressionProvider
    {
        public byte[] Compress(byte[] data)
        {
            var output = new MemoryStream();
            using var stream = new DeflateStream(output, CompressionLevel.Fastest);
            stream.Write(data, 0, data.Length);
            stream.Flush();
            return output.ToArray();
        }

        public byte[] Decompress(byte[] compressed)
        {
            var input = new MemoryStream(compressed);
            var output = new MemoryStream();
            using var stream = new DeflateStream(input, CompressionMode.Decompress);
            stream.CopyTo(output);
            return output.ToArray();
        }
    }
}