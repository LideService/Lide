using System;
using System.IO;
using System.Threading.Tasks;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;

namespace Lide.Core.Provider;

public class StreamBatchProvider : IStreamBatchProvider
{
    public async Task WriteNextBatch(Stream stream, byte[] data)
    {
        await stream.WriteAsync(BitConverter.GetBytes(data.Length)).ConfigureAwait(false);
        await stream.WriteAsync(data).ConfigureAwait(false);
        await stream.FlushAsync().ConfigureAwait(false);
    }

    public async Task<BinaryBatchData> NextBatch(Stream stream, int startPosition)
    {
        stream.Seek(startPosition, SeekOrigin.Begin);
        var buffer = new byte[sizeof(int)];
        var read = await stream.ReadAsync(buffer.AsMemory(0, sizeof(int))).ConfigureAwait(false);
        if (read == 0)
        {
            return new BinaryBatchData { Data = Array.Empty<byte>(), EndPosition = -1 };
        }

        var length = BitConverter.ToInt32(buffer);
        var data = new byte[length];
        _ = await stream.ReadAsync(data.AsMemory(0, length)).ConfigureAwait(false);
        return new BinaryBatchData
        {
            Data = data,
            EndPosition = sizeof(int) + data.Length + startPosition,
        };
    }
}