using System.Threading.Tasks;
using Lide.Core.Model;

namespace Lide.Core.Contract.Facade
{
    public interface IFileFacade
    {
        Task WriteToFile(string filePath, byte[] data);
        Task<BinaryFileBatch> ReadNextBatch(string filePath, int startPosition = 0);
        void DeleteFile(string filePath);
    }
}