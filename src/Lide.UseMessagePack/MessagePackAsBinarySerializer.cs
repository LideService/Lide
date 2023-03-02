using System.IO;
using Lide.Core.Contract.Provider;
using MessagePack;

namespace Lide.UseMessagePack;

public class MessagePackAsBinarySerializer : IBinarySerializeProvider
{
    public byte[] Serialize(object data)
    {
        var serialized = MessagePackSerializer.Typeless.Serialize(data);
        return serialized;
    }

    public object Deserialize(byte[] data)
    {
        var deserialized = MessagePackSerializer.Typeless.Deserialize(data);
        return deserialized;
    }

    public T Deserialize<T>(byte[] data)
    {
        var deserialized = MessagePackSerializer.Typeless.Deserialize(data);
        return (T)deserialized;
    }
}