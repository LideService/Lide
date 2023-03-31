using Lide.Demo.Exchange.Core.Contracts;
using Lide.Demo.Exchange.Core.Service;
using Lide.Demo.Exchange.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lide.Demo.Exchange.WebAPI;

public static class Configuration
{
    public static WebHostBuilder GetBuilder()
    {
        var builder = new WebHostBuilder();
        builder.ConfigureServices((services) =>
        {
            // services.AddLide();
            services.AddRouting();
            services.AddScoped<IMapper, Mapper>();
            services.AddScoped<IDateTimeFacade, DateTimeFacade>();
            services.AddScoped<ISeedDataGenerator, SeedDataGenerator>();
            services.AddScoped<IExchangeService, ExchangeService>();
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
            endpointBuilder.MapPost(Endpoints.AddCurrency, ([FromBody] Currency currency, IExchangeService exchangeService) => exchangeService.AddCurrency(currency));
            endpointBuilder.MapPost(Endpoints.AddExchangeRate, ([FromBody] ExchangeData exchangeData, IExchangeService exchangeService) => exchangeService.AddExchangeRate(exchangeData));
            endpointBuilder.MapPost(Endpoints.GetCurrency, ([FromBody] Currency currency, IExchangeService exchangeService) => exchangeService.GetCurrency(currency.Code));
            endpointBuilder.MapPost(Endpoints.GetExchangeRate, ([FromBody] GetExchangeRate exchangeData, IExchangeService exchangeService) => exchangeService.GetExchangeRateWithThirdOptional(exchangeData.FromCurrency.Code, exchangeData.ToCurrency.Code, exchangeData.ExchangeDate!.Value));
            endpointBuilder.MapPost(Endpoints.GetExchangeRates, ([FromBody] GetExchangeRate exchangeData, IExchangeService exchangeService) => exchangeService.GetExchangeRates(exchangeData.FromCurrency, exchangeData.ToCurrency));
            endpointBuilder.MapPost(Endpoints.GetExchangeRatesWithDateRange, ([FromBody] GetExchangeRate exchangeData, IExchangeService exchangeService) => exchangeService.GetExchangeRates(exchangeData.FromCurrency, exchangeData.ToCurrency, exchangeData.StartDate!.Value, exchangeData.EndDate!.Value));

            endpointBuilder.MapPost(Endpoints.GetDefaultCurrencies, (ISeedDataGenerator seedDataGenerator) => seedDataGenerator.GetPredefinedCurrencies());
            endpointBuilder.MapPost(Endpoints.GetAdditionalCurrencies, (ISeedDataGenerator seedDataGenerator) => seedDataGenerator.GetAnotherSetOfCurrencies());
            endpointBuilder.MapPost(Endpoints.GetSeedExchangeRates, ([FromBody] GetExchangeRate exchangeData, ISeedDataGenerator seedDataGenerator) => seedDataGenerator.GetExchangeRates(exchangeData.FromCurrency, exchangeData.ToCurrency, exchangeData.StartDate!.Value, exchangeData.EndDate!.Value));
        });
    }
}