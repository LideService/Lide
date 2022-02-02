using System.Net.Http;

namespace Lide.WebApi.Contract
{
    public interface IHttpClientRebuilder
    {
        HttpClient RebuildNewClient(HttpClient originalClient);
    }
}