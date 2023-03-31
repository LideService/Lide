using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Lide.Demo.Reporting.Model;

namespace Lide.Demo.Reporting.Client;

public class SeedClient
{
    private readonly HttpClient _httpClient;

    public SeedClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TaxLevel[]> GetRandomTaxLevels()
    {
        var response = await _httpClient.PostAsync(Endpoints.GetRandomSeed, null).ConfigureAwait(false);
        var result = await response.Content.ReadFromJsonAsync<TaxLevel[]>().ConfigureAwait(false);
        return result ?? Array.Empty<TaxLevel>();
    }

    public async Task<TaxLevel[]> GetPredefinedTaxLevels()
    {
        var response = await _httpClient.PostAsync(Endpoints.GetDefaultSeed, null).ConfigureAwait(false);
        var result = await response.Content.ReadFromJsonAsync<TaxLevel[]>().ConfigureAwait(false);
        return result ?? Array.Empty<TaxLevel>();
    }
}