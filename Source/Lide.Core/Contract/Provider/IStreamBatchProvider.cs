using System.IO;
using System.Threading.Tasks;
using Lide.Core.Model;

namespace Lide.Core.Contract.Provider
{
    public interface IStreamBatchProvider
    {
        Task WriteNextBatch(Stream stream, byte[] data);
        Task<BinaryBatchData> NextBatch(Stream stream, int startPosition);
    }
}