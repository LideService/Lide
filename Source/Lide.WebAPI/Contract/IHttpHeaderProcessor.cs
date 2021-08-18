using System.Net.Http;

namespace Lide.WebAPI.Contract
{
    public interface IHttpHeaderProcessor
    {
        void AddHeaders(HttpClient httpClient);
    }
}