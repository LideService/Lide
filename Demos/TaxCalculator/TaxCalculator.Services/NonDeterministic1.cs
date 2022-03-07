using System.Net.Http;
using System.Text.Json;
using TaxCalculator.Services.Contracts;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Services
{
    public class NonDeterministic1 : INonDeterministic1
    {
        private readonly INonDeterministic2 _nonDeterministic2;
        private readonly HttpClient _httpClient;

        public NonDeterministic1(INonDeterministic2 nonDeterministic2, HttpClient httpClient)
        {
            _nonDeterministic2 = nonDeterministic2;
            _httpClient = httpClient;
        }

        public BadDesignData InitializeAndDoStuff_NoIsolation(decimal input)
        {
            var data = new BadDesignData()
            {
                Field1 = input * 3 + 13,
                Field2 = input + 19,
            };

            _nonDeterministic2.Volatile_TreatInputAsNonMutable_NoIsolation(data);
            data.Field1 += 1;
            data.Field2 += 2;
            return data;
        }

        public BadDesignData InitializeAndDoStuff_NoIsolationWithInnerCall(decimal input)
        {
            var data = new BadDesignData()
            {
                Field1 = input * 3 + 13,
                Field2 = input + 19,
            };

            _nonDeterministic2.Volatile_TreatInputAsNonMutable_NoIsolation(data);
            var result = _httpClient.PostAsync("https://localhost:5001/nond/withisolation?initialValue=132", new StringContent("")).Result;
            var resultContent = result.Content.ReadAsStringAsync().Result;
            var resultObject = JsonSerializer.Deserialize<BadDesignData>(resultContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            data.Field1 += resultObject.Field1;
            data.Field2 += resultObject.Field2;
            return data;
        }

        public BadDesignData InitializeAndDoStuff_WithIsolation(decimal input)
        {
            var data = new BadDesignData()
            {
                Field1 = input * 3 + 13,
                Field2 = input + 19,
            };

            _nonDeterministic2.Volatile_TreatInputAsNonMutable_WithIsolation(data);
            data.Field1 += 1;
            data.Field2 += 2;
            return data;
        }
    }
}