using System;

namespace Lide.Core.Contract.Provider
{
    public interface ISerializeProvider
    {
        string SerializeToString(object data);
        byte[] Serialize(object data);
        object Deserialize(byte[] data);
        T Deserialize<T>(byte[] data);
    }
}