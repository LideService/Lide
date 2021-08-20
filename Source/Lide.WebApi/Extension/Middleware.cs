using Microsoft.AspNetCore.Builder;

namespace Lide.WebApi.Extension
{
    public static class Middleware
    {
        public static IApplicationBuilder UseLide(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ContainerMiddleware>();
            return builder;
        }
    }
}