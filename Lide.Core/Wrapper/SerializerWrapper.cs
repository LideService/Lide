using System;
using System.Text.Json;
using Lide.Core.Contract.Wrapper;

namespace Lide.Core.Wrapper
{
    public class SerializerWrapper : ISerializerWrapper
    {
        public string Serialize<T>(T data)
        {
            return JsonSerializer.Serialize(data);
        }

        public T Deserialize<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        public object Deserialize(string data, Type type)
        {
            return JsonSerializer.Deserialize(data, type);
        }
    }
}