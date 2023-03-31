using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lide.Demo.Taxes.Client;
using Lide.Demo.Taxes.Model;
using Lide.Demo.Taxes.WebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Demo.Taxes.Test;

[TestClass]
public class TestWebAPI
{
    [TestMethod]
    public async Task FullScenario_WithDefaultSeed()
    {
        var client = GetClient();
        var seedClient = new SeedClient(client);
        var taxClient = new TaxClient(client);
        var calculatorClient = new CalculatorClient(client);

        var defaultTaxLevel = await seedClient.GetPredefinedTaxLevels().ConfigureAwait(false);
        var taxNames = defaultTaxLevel.Select(x => new TaxName { Value = x.Name }).ToArray();
        await taxClient.AddTaxLevels(defaultTaxLevel).ConfigureAwait(false);

        var calculateRequest1 = new CalculateRequest { Amount = 1950, TaxNames = taxNames };
        var calculateRequest2 = new CalculateRequest { Amount = 2700, TaxNames = taxNames };

        var calculateResponse1 = await calculatorClient.CalculateAfterTax(calculateRequest1).ConfigureAwait(false);
        var calculateResponse2 = await calculatorClient.CalculateAfterTax(calculateRequest2).ConfigureAwait(false);

        Assert.AreEqual(1883.50m, calculateResponse1);
        Assert.AreEqual(2346.00m, calculateResponse2);
    }

    [TestMethod]
    public async Task FullScenario_WithRandomSeed()
    {
        var client = GetClient();
        var seedClient = new SeedClient(client);
        var taxClient = new TaxClient(client);
        var calculatorClient = new CalculatorClient(client);

        var defaultTaxLevel = await seedClient.GetRandomTaxLevels().ConfigureAwait(false);
        var taxNames = defaultTaxLevel.Select(x => new TaxName { Value = x.Name }).ToArray();
        await taxClient.AddTaxLevels(defaultTaxLevel).ConfigureAwait(false);

        var calculateRequest1 = new CalculateRequest { Amount = 1950, TaxNames = taxNames };
        var calculateRequest2 = new CalculateRequest { Amount = 2700, TaxNames = taxNames };

        var calculateResponse1 = await calculatorClient.CalculateAfterTax(calculateRequest1).ConfigureAwait(false);
        var calculateResponse2 = await calculatorClient.CalculateAfterTax(calculateRequest2).ConfigureAwait(false);

        Assert.AreNotEqual(-1, calculateResponse1);
        Assert.AreNotEqual(-1, calculateResponse2);
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