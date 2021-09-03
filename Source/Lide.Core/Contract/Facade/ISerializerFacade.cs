using System;

namespace Lide.Core.Contract.Facade
{
    public interface ISerializerFacade
    {
         byte[] Serialize<T>(T data);
         string SerializeString<T>(T data);
         T Deserialize<T>(byte[] data);
         object Deserialize(byte[] data, Type type);
    }
}