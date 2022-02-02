using System.Threading.Tasks;

namespace Lide.Core.Contract.Provider
{
    public interface IPropagateContentController
    {
        Task<byte[]> GetDataForRequest();
        Task PutDataFromRequest(byte[] data);

        Task<byte[]> GetDataForResponse();
        Task PutDataFromResponse(byte[] data);
    }
}