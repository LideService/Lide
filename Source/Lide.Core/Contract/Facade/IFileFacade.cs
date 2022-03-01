using System.IO;
using System.Threading.Tasks;
using Lide.Core.Model;

namespace Lide.Core.Contract.Facade
{
    public interface IFileFacade
    {
        string GetFileName(string id = null);
        Stream OpenFile(string filePath);
        void DeleteFile(string filePath);
        Task<byte[]> ReadWholeFle(string filePath);
        Task WriteWholeFile(string filePath, byte[] data);
    }
}