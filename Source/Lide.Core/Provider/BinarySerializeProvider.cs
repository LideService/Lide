using Lide.Core.Contract.Provider;
using Lide.Core.Provider.Internal;

namespace Lide.Core.Provider
{
    public class BinarySerializeProvider : IBinarySerializeProvider
    {
        private readonly ObjectConverter _typeConverter;

        public BinarySerializeProvider()
        {
            _typeConverter = new ObjectConverter();
        }

        public byte[] Serialize(object data)
        {
            return _typeConverter.Serialize(data);
        }

        public object Deserialize(byte[] data)
        {
            return _typeConverter.Deserialize(data);
        }

        public T Deserialize<T>(byte[] data)
        {
            return (T)_typeConverter.Deserialize(data);
        }
    }
}