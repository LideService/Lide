using System;
using System.Threading.Tasks;
using Lide.Core.Facade;
using Microsoft.AspNetCore.Http;

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
            var headers = httpContext.Request.Headers;
            var lideCompression = headers.ContainsKey("Lide.Compression") && Convert.ToBoolean(headers["Lide.Compression"]);
            var lideEnabled = headers.ContainsKey("Lide.Enable") && Convert.ToBoolean(headers["Lide.Enable"]);

            // TODO
            if (lideEnabled)
            {
            }
            
            httpContext.RequestServices = new ServiceProviderWrapper(httpContext.RequestServices);
            await _next(httpContext); 
        }
    }
}