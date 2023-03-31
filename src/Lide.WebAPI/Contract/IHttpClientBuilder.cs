using System.Net.Http;

namespace Lide.WebAPI.Contract;

public interface IHttpClientBuilder
{
    HttpClient RebuildNewClient(HttpClient originalClient);
}