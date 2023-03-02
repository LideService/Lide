namespace Lide.Core.Contract.Provider;

public interface IBinarySerializeProvider
{
    byte[] Serialize(object data);
    object Deserialize(byte[] data);
    T Deserialize<T>(byte[] data);
}