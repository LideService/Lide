using Microsoft.AspNetCore.Builder;

namespace Lide.WebApi.Extension
{
    public static class AddMiddleware
    {
        public static IApplicationBuilder UseLide(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<LideMiddleware>();
            return builder;
        }
    }
}