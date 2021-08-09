using System;
using Lide.WebAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            
            services.AddHttpClient("github", c =>
            {
                Console.WriteLine("HttpClient");
                c.BaseAddress = new Uri("https://api.github.com/");
                // Github API versioning
                c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                // Github requires a user-agent
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider sp)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lide.WebApiTests v1"));
            }
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<ContainerMiddleware>();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}