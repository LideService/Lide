using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class CompressionProvider : ICompressionProvider
    {
        public string CompressString(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
            gZipStream.Write(buffer, 0, buffer.Length);

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            
            return Convert.ToBase64String(gZipBuffer);
        }

        public string DecompressString(string compressedText)
        {
            var gZipBuffer = Convert.FromBase64String(compressedText);
            var dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            var buffer = new byte[dataLength];
            
            using var memoryStream = new MemoryStream();
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
            gZipStream.Read(buffer, 0, buffer.Length);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}