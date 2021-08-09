using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Lide.Decorators.DataProcessors;
using Lide.Decorators.Wrappers;
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

            if (lideEnabled)
            {
            }
            
            httpContext.RequestServices = new ServiceProviderWrapper(provider);
            await _next(httpContext); 
        }
    }
}