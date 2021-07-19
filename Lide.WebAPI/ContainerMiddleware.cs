using System;
using System.Threading.Tasks;
using Lide.Decorators.DataProcessors;
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
            httpContext.RequestServices = new ServiceProviderWrapper(provider);
            await _next(httpContext);
        }
    }
}