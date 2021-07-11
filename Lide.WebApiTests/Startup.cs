using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Lide.WebApiTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Lide.WebApiTests", Version = "v1"}); });
            //services.AddSingleton<IKarta, Obb>();
            //services.AddSingleton<IKarta2, Wrapper<Dsk>>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider sp)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lide.WebApiTests v1"));
            }

            app.UseMiddleware<ReplaceMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private class ReplaceMiddleware
        {
            private readonly RequestDelegate _next;

            public ReplaceMiddleware(RequestDelegate next)
            {
                _next = next;
            }
            public Task Invoke(HttpContext httpContext)
            {
                Console.WriteLine("Invoke");
                var provider = httpContext.RequestServices.GetService<IServiceProvider>();
                httpContext.RequestServices = new ContainerBuilder(provider);
                return this._next(httpContext);
            }
        }
    }
}

/*
 * 1. Потрфейл
 * 2. Карта
 * 3. Чекиране
 * 4. Пин
 * 5. .. 
*/