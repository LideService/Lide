using System.IO;
using Lide.BinarySerialization.Framework;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider;

public class BinarySerializeProvider : IBinarySerializeProvider
{
    public byte[] Serialize(object data)
    {
        var formatter = new BinaryFormatter();
        using var ms = new MemoryStream();
        formatter.Serialize(ms, data);
        return ms.ToArray();
    }

    public object Deserialize(byte[] data)
    {
        var formatter = new BinaryFormatter();
        using var ms = new MemoryStream(data);
        return formatter.Deserialize(ms);
    }

    public T Deserialize<T>(byte[] data)
    {
        var formatter = new BinaryFormatter();
        using var ms = new MemoryStream(data);
        return (T)formatter.Deserialize(ms);
    }
}