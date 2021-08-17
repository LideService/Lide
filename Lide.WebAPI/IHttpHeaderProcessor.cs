using System.Net.Http;

namespace Lide.WebAPI
{
    public interface IHttpHeaderProcessor
    {
        void AddHeaders(HttpClient httpClient);
    }
}