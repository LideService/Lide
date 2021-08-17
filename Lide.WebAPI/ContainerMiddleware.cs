using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebAPI
{
    public class ContainerMiddleware
    {
        private readonly RequestDelegate _next;

        public ContainerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task Invoke(HttpContext httpContext)
        {
            var provider = httpContext.RequestServices.GetService<IServiceProvider>();
            var headers = httpContext.Request.Headers;
            var lideCompression = headers.ContainsKey("Lide.Compression") && Convert.ToBoolean(headers["Lide.Compression"]);
            var lideEnabled = headers.ContainsKey("Lide.Enable") && Convert.ToBoolean(headers["Lide.Enable"]);

            // TODO
            if (lideEnabled)
            {
            }
            
            // plugins!
            // if (originalObject is HttpClient httpClient)
            // {
            //     httpClient.DefaultRequestHeaders.Add("","");
            //     _httpHeaderProcessor.AddHeaders(httpClient);
            // }
            //
            // if (serviceType == typeof(IHttpClientFactory))
            // {
            //     var wrapper = new HttpClientFactoryWrapper(originalObject as IHttpClientFactory, _httpHeaderProcessor);
            //     _generatedProxies.Add(originalObject, wrapper);
            //     return wrapper;
            // }
            
            httpContext.RequestServices = new ServiceProviderWrapper(provider);
            await _next(httpContext); 
        }
    }
}