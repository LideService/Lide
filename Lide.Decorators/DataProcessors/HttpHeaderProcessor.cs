using System.Net.Http;
using Lide.Decorators.Contract;

namespace Lide.Decorators.DataProcessors
{
    public class HttpHeaderProcessor : IHttpHeaderProcessor
    {
        private readonly ILideSettingsProcessor _lideSettingsProcessor;

        public HttpHeaderProcessor(ILideSettingsProcessor lideSettingsProcessor)
        {
            _lideSettingsProcessor = lideSettingsProcessor;
        }
        
        public void AddHeaders(HttpClient httpClient)
        {
            // TODO
            httpClient.DefaultRequestHeaders.Add("Lide.Compression","true");
            httpClient.DefaultRequestHeaders.Add("Lide.Enable","");
            httpClient.DefaultRequestHeaders.Add("Lide.Settings","");
        }
    }
}