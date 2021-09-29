using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class JsonSerializeProvider : IJsonSerializeProvider
    {
        public string Serialize(object data)
        {
            return System.Text.Json.JsonSerializer.Serialize(data);
        }

        public T Deserialize<T>(string data)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(data);
        }
    }
}