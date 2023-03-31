using System;
using System.Net.Http;
using System.Net.Http.Json;
using Lide.Demo.Exchange.Model;

namespace Lide.Demo.Exchange.Client;

public class SeedClient
{
    private readonly HttpClient _httpClient;

    public SeedClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Currency[] GetDefaultCurrencies()
    {
        var response = _httpClient.GetAsync(Endpoints.GetDefaultCurrencies).Result;
        return response.IsSuccessStatusCode
            ? response.Content.ReadFromJsonAsync<Currency[]>().Result ?? Array.Empty<Currency>()
            : Array.Empty<Currency>();
    }

    public Currency[] GetAnotherSetOfCurrencies()
    {
        var response = _httpClient.GetAsync(Endpoints.GetAdditionalCurrencies).Result;
        return response.IsSuccessStatusCode
            ? response.Content.ReadFromJsonAsync<Currency[]>().Result ?? Array.Empty<Currency>()
            : Array.Empty<Currency>();
    }

    public ExchangeData[] GetExchangeRates(Currency fromCurrency, Currency toCurrency, DateTime startDate, DateTime endDate)
    {
        var body = new GetExchangeRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            ExchangeDate = null,
            StartDate = startDate,
            EndDate = endDate,
        };
        var response = _httpClient.PostAsJsonAsync(Endpoints.GetExchangeRates, body).Result;
        return response.IsSuccessStatusCode
            ? response.Content.ReadFromJsonAsync<ExchangeData[]>().Result ?? Array.Empty<ExchangeData>()
            : Array.Empty<ExchangeData>();
    }
}