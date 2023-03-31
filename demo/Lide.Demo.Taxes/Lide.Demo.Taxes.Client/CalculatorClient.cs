using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Lide.Demo.Taxes.Model;

namespace Lide.Demo.Taxes.Client;

public class CalculatorClient
{
    private readonly HttpClient _httpClient;

    public CalculatorClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> CalculateAfterTax(CalculateRequest input)
    {
        var response = await _httpClient.PostAsJsonAsync(Endpoints.CalculateTax, input).ConfigureAwait(false);
        var result = await response.Content.ReadFromJsonAsync<CalculateResponse>().ConfigureAwait(false);
        return result?.Value ?? -1;
    }
}