using Microsoft.AspNetCore.Builder;

namespace Lide.WebAPI
{
    public static class AppBuilderExtension
    {
        public static IApplicationBuilder UseLide(IApplicationBuilder builder)
        {
            return builder;
        }
    }
}