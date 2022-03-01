using System.Text.Json;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class JsonSerializeProvider : IJsonSerializeProvider
    {
        public string Serialize(object data)
        {
            return JsonSerializer.Serialize(data);
        }

        public T Deserialize<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}