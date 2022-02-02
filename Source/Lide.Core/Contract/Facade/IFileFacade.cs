using System.Threading.Tasks;
using Lide.Core.Model;

namespace Lide.Core.Contract.Facade
{
    public interface IFileFacade
    {
        string GetFileName(string id = null);
        Task WriteNextBatch(string filePath, byte[] data);
        Task<BinaryFileBatch> ReadNextBatch(string filePath, int startPosition = 0);
        void DeleteFile(string filePath);
        Task<byte[]> ReadWholeFle(string filePath);
        Task WriteWholeFile(string filePath, byte[] data);
    }
}