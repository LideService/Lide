using System;
using Lide.Core.Contract.Provider;
using Lide.Core.Provider.Internal;

namespace Lide.Core.Provider
{
    public class SerializeProvider : ISerializeProvider
    {
        private readonly TypeConverterProvider _converter;

        public SerializeProvider()
        {
            _converter = new TypeConverterProvider();
        }

        public string SerializeToString(object data)
        {
            return System.Text.Json.JsonSerializer.Serialize(data);
        }

        public byte[] Serialize(object data)
        {
            var converter = _converter.GenerateReadAction(data.GetType());
            return converter(data);
        }

        public object Deserialize(Type type, byte[] data)
        {
            var converter = _converter.GenerateWriteAction(type);
            return converter(data);
        }

        public T Deserialize<T>(byte[] data)
        {
            var converter = _converter.GenerateWriteAction(typeof(T));
            return (T)converter(data);
        }
    }
}