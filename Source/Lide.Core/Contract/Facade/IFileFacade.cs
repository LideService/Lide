using System.Threading.Tasks;

namespace Lide.Core.Contract.Facade
{
    public interface IFileFacade
    {
        Task WriteToFile(string filePath, byte[] data);
        Task<(byte[] data, int endPosition)> ReadNextBatch(string filePath, int startPosition = 0);
        Task<byte[]> ReadAllBytes(string filePath);
    }
}