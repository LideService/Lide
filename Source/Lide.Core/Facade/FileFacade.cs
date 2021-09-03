using System;
using System.IO;
using System.Threading.Tasks;
using Lide.Core.Contract.Facade;

namespace Lide.Core.Facade
{
    public class FileFacade : IFileFacade
    {
        public async Task WriteToFile(string filePath, byte[] data)
        {
            await using var fileHandle = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None);
            await fileHandle.WriteAsync(BitConverter.GetBytes(data.Length)).ConfigureAwait(false);
            await fileHandle.WriteAsync(data).ConfigureAwait(false);
            await fileHandle.FlushAsync().ConfigureAwait(false);
        }

        public async Task<(byte[] data, int endPosition)> ReadNextBatch(string filePath, int startPosition = 0)
        {
            await using var fileHandle = File.OpenRead(filePath);
            fileHandle.Seek(startPosition, SeekOrigin.Begin);
            var buffer = new byte[sizeof(int)];
            var read = await fileHandle.ReadAsync(buffer.AsMemory(0, sizeof(int))).ConfigureAwait(false);
            if (read == 0)
            {
                return (null, -1);
            }

            var length = BitConverter.ToInt32(buffer);
            var data = new byte[length];
            read = await fileHandle.ReadAsync(data.AsMemory(0, length)).ConfigureAwait(false);
            Console.WriteLine(read);
            return (data, sizeof(int) + length + startPosition);
        }

        public Task<byte[]> ReadAllBytes(string filePath)
        {
            return File.ReadAllBytesAsync(filePath);
        }
    }
}