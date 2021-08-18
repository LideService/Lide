using Microsoft.AspNetCore.Builder;

namespace Lide.WebAPI.Extension
{
    public static class AppBuilderExtension
    {
        public static IApplicationBuilder UseLide(IApplicationBuilder builder)
        {
            return builder;
        }
    }
}