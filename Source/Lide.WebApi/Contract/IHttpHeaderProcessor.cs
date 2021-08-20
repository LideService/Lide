using System.Net.Http;

namespace Lide.WebApi.Contract
{
    public interface IHttpHeaderProcessor
    {
        void AddHeaders(HttpClient httpClient);
    }
}