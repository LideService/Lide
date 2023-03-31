using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lide.Demo.Exchange.Client;
using Lide.Demo.Exchange.Model;
using Lide.Demo.Exchange.WebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Demo.Exchange.Test;

[TestClass]
public class TestWebAPI
{
    [TestMethod]
    public void FullScenario_WithDefaultSeed()
    {
        var client = GetClient();
        var exchangeClient = new ExchangeClient(client);
        var currencies = SeedData(client, exchangeClient);

        var exchangeRate1 = exchangeClient.GetExchangeRate(currencies[0], currencies[1], DateTime.Today);
        var exchangeRate2 = exchangeClient.GetExchangeRate(currencies[1], currencies[0], DateTime.Today);

        Assert.AreEqual(exchangeRate1.ExchangeRate, 1 / exchangeRate2.ExchangeRate);
    }

    private static Currency[] SeedData(HttpClient client, ExchangeClient exchangeClient)
    {
        var seedClient = new SeedClient(client);

        var currencies = seedClient.GetDefaultCurrencies();
        var currencyPairs = currencies.SelectMany(c1 => currencies.Select(c2 => (c1, c2)))
            .Where(pair => pair.c1 != pair.c2)
            .ToArray();

        foreach (var currency in currencies)
        {
            exchangeClient.AddCurrency(currency);
        }

        var seedExchangeRates = currencyPairs
            .Select(pair => seedClient.GetExchangeRates(pair.c1, pair.c2, DateTime.Today.AddYears(-1), DateTime.Today))
            .SelectMany(exchangeRates => exchangeRates)
            .ToArray();

        foreach (var exchangeData in seedExchangeRates)
        {
            exchangeClient.AddExchangeRate(exchangeData);
        }

        return currencies;
    }

    private static HttpClient GetClient()
    {
        var builder = Configuration.GetBuilder();
        builder.UseTestServer();
        builder.Configure(app => app.ConfigureApp());
        var webHost = builder.Build();

        webHost.Start();
        return webHost.GetTestClient();
    }
}