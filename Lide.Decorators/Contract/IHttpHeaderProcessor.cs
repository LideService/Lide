using System.Net.Http;

namespace Lide.Decorators.Contract
{
    public interface IHttpHeaderProcessor
    {
        void AddHeaders(HttpClient httpClient);
    }
}