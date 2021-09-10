using System;

namespace Lide.Core.Contract.Provider
{
    public interface ISerializeProvider
    {
        string SerializeToString(object data);
        byte[] Serialize(object data);
        object Deserialize(Type type, byte[] data);
        T Deserialize<T>(byte[] data);
    }
}