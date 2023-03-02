using System.Net.Http;

namespace Lide.WebApi.Contract;

public interface IHttpClientBuilder
{
    HttpClient RebuildNewClient(HttpClient originalClient);
}