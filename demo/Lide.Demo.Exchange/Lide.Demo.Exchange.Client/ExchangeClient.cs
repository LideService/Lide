using System;
using System.Net.Http;
using System.Net.Http.Json;
using Lide.Demo.Exchange.Core.Contracts;
using Lide.Demo.Exchange.Model;

namespace Lide.Demo.Exchange.Client;

public class ExchangeClient
{
    private readonly HttpClient _httpClient;

    public ExchangeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void AddCurrency(Currency currency)
    {
        _httpClient.PostAsJsonAsync(Endpoints.AddCurrency, currency).Wait();
    }

    public void AddExchangeRate(ExchangeData exchangeData)
    {
        _httpClient.PostAsJsonAsync(Endpoints.AddExchangeRate, exchangeData).Wait();
    }

    public Currency? GetCurrency(Currency currency)
    {
        var response = _httpClient.PostAsJsonAsync(Endpoints.GetCurrency, currency).Result;
        return response.IsSuccessStatusCode
            ? response.Content.ReadFromJsonAsync<Currency>().Result
            : null;
    }

    public ExchangeData? GetExchangeRate(Currency fromCurrency, Currency toCurrency, DateTime exchangeDate)
    {
        var body = new GetExchangeRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            ExchangeDate = exchangeDate,
            StartDate = null,
            EndDate = null,
        };
        var response = _httpClient.PostAsJsonAsync(Endpoints.GetExchangeRate, body).Result;
        return response.IsSuccessStatusCode
            ? response.Content.ReadFromJsonAsync<ExchangeData>().Result
            : null;
    }

    public ExchangeData[] GetRawExchangeRates(Currency fromCurrency, Currency toCurrency)
    {
        var body = new GetExchangeRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            ExchangeDate = null,
            StartDate = null,
            EndDate = null,
        };
        var response = _httpClient.PostAsJsonAsync(Endpoints.GetExchangeRates, body).Result;
        return response.IsSuccessStatusCode
            ? response.Content.ReadFromJsonAsync<ExchangeData[]>().Result ?? Array.Empty<ExchangeData>()
            : Array.Empty<ExchangeData>();
    }

    public ExchangeData[] GetRawExchangeRates(Currency fromCurrency, Currency toCurrency, DateTime startDate, DateTime endDate)
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