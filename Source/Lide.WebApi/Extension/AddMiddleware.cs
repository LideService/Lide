using Microsoft.AspNetCore.Builder;

namespace Lide.WebApi.Extension
{
    public static class AddMiddleware
    {
        public static IApplicationBuilder UseLide(this IApplicationBuilder builder)
        {
            //// builder.UseEndpoints(endpoints =>
            //// {
            ////     endpoints.MapControllerRoute(
            ////         name: "default",
            ////         pattern: "{controller=Home}/{action=Index}/{id?}");
            //// });

            builder.UseMiddleware<LideMiddleware>();
            return builder;
        }
    }
}