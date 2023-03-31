using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Lide.Demo.Taxes.Model;

namespace Lide.Demo.Taxes.Client;

public class TaxClient
{
    private readonly HttpClient _httpClient;

    public TaxClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AddTaxLevel(TaxLevel taxLevel)
    {
        _ = await _httpClient.PostAsJsonAsync(Endpoints.AddSingleTax, taxLevel).ConfigureAwait(false);
    }

    public async Task AddTaxLevels(TaxLevel[] taxLevels)
    {
        _ = await _httpClient.PostAsJsonAsync(Endpoints.AddManyTaxes, taxLevels).ConfigureAwait(false);
    }

    public async Task<TaxLevel> GetTaxByName(TaxName name)
    {
        var response = await _httpClient.PostAsJsonAsync(Endpoints.GetTaxByName, name).ConfigureAwait(false);
        var result = await response.Content.ReadFromJsonAsync<TaxLevel>().ConfigureAwait(false);
        return result ?? new TaxLevel("Not found", null, null, 0);
    }

    public async Task<TaxLevel[]> GetAllTaxes()
    {
        var response = await _httpClient.PostAsync(Endpoints.GetAllTaxes, null).ConfigureAwait(false);
        var result = await response.Content.ReadFromJsonAsync<TaxLevel[]>().ConfigureAwait(false);
        return result ?? Array.Empty<TaxLevel>();
    }
}