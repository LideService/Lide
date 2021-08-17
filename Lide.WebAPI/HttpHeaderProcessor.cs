using System.Net.Http;

namespace Lide.WebAPI
{
    public class HttpHeaderProcessor2 : IHttpHeaderProcessor
    {
        private readonly ISettingsProvider _settingsProvider;

        public HttpHeaderProcessor2(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
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