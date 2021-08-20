using Microsoft.AspNetCore.Builder;

namespace Lide.WebAPI.Extension
{
    public static class Middleware
    {
        public static IApplicationBuilder UseLide(IApplicationBuilder builder)
        {
            builder.UseMiddleware<ContainerMiddleware>();
            return builder;
        }
    }
}