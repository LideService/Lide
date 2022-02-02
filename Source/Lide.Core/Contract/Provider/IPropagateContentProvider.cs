using System.Threading.Tasks;

namespace Lide.Core.Contract.Provider
{
    public interface IPropagateContentProvider
    {
        public void PutFileForRequest(string name, string filePath);
        public void PutDataForRequest(string name, byte[] data);

        public string ReadFileFromRequest(string name);
        public byte[] ReadDataFromRequest(string name);

        public void PutFileForResponse(string name, string filePath);
        public void PutDataForResponse(string name, byte[] data);

        public string ReadFileFromResponse(string name);
        public byte[] ReadDataFromResponse(string name);
    }
}