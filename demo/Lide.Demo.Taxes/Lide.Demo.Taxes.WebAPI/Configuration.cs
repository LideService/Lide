using Lide.Demo.Taxes.Core.Contracts;
using Lide.Demo.Taxes.Core.Service;
using Lide.Demo.Taxes.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lide.Demo.Taxes.WebAPI;

public static class Configuration
{
    public static WebHostBuilder GetBuilder()
    {
        var builder = new WebHostBuilder();
        builder.ConfigureServices((services) =>
        {
            // services.AddLide();
            services.AddRouting();
            services.AddScoped<ICalculator, Calculator>();
            services.AddScoped<IDateTimeFacade, DateTimeFacade>();
            services.AddScoped<IRandomFacade, RandomFacade>();
            services.AddScoped<ISeedDataGenerator, SeedDataGenerator>();
            services.AddScoped<ITaxLevelsState, TaxLevelsState>();
        });

        builder.ConfigureLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        builder.UseKestrel();
        return builder;
    }

    public static void ConfigureApp(this IApplicationBuilder app)
    {
        // app.UseLide();
        _ = app.UseRouting();
        _ = app.UseHttpsRedirection();
        _ = app.UseEndpoints(endpointBuilder =>
        {
            endpointBuilder.MapPost(Endpoints.AddSingleTax, ([FromBody] TaxLevel taxLevel, ITaxLevelsState taxLevelsState) => taxLevelsState.AddTaxLevel(taxLevel));
            endpointBuilder.MapPost(Endpoints.AddManyTaxes, ([FromBody] TaxLevel[] taxLevels, ITaxLevelsState taxLevelsState) => taxLevelsState.AddTaxLevels(taxLevels));
            endpointBuilder.MapPost(Endpoints.GetAllTaxes, (ITaxLevelsState taxLevelsState) => taxLevelsState.GetTaxes());
            endpointBuilder.MapPost(Endpoints.GetTaxByName, ([FromBody] TaxName taxName, ITaxLevelsState taxLevelsState) => taxLevelsState.GetTaxByName(taxName.Value));

            endpointBuilder.MapPost(Endpoints.GetRandomSeed, (ISeedDataGenerator taxLevelsState) => taxLevelsState.GetRandomTaxLevels());
            endpointBuilder.MapPost(Endpoints.GetDefaultSeed, (ISeedDataGenerator taxLevelsState) => taxLevelsState.GetPredefinedTaxLevels());

            endpointBuilder.MapPost(Endpoints.CalculateTax, ([FromBody] CalculateRequest input, ITaxLevelsState taxLevelsState, ICalculator calculator) =>
            {
                var taxes = taxLevelsState.GetTaxesByName(input.TaxNames);
                var result = calculator.CalculateAfterTax(input.Amount, taxes);
                return new CalculateResponse { Value = result };
            });
        });
    }
}