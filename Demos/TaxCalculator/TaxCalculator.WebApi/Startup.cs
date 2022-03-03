using Lide.WebApi.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TaxCalculator.Services;
using TaxCalculator.Services.Contracts;

namespace TaxCalculator.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaxCalculator.WebApi", Version = "v1" });
                c.OperationFilter<EnableSwaggerHeaders>();
            });
            services.AddLideCore(Configuration);
            services.AddSingleton<ICalculator, Calculator>();
            services.AddSingleton<ITaxLevelsState, TaxLevelsState>();
            services.AddScoped<INonDeterministic1, NonDeterministic1>();
            services.AddScoped<INonDeterministic2, NonDeterministic2>();
            services.AddScoped<IDateTimeFacade, DateTimeFacade>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaxCalculator.WebApi v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseLide();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}