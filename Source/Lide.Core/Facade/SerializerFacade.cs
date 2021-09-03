using System;
using System.Text.Json;
using Lide.Core.Contract.Facade;

namespace Lide.Core.Facade
{
    public class SerializerFacade : ISerializerFacade
    {
        public byte[] Serialize<T>(T data)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data);
        }

        public string SerializeString<T>(T data)
        {
            return JsonSerializer.Serialize(data);
        }

        public T Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        public object Deserialize(byte[] data, Type type)
        {
            return JsonSerializer.Deserialize(data, type);
        }
    }
}