using System;

namespace Lide.Core.Contract.Facade
{
    public interface ISerializerFacade
    {
         byte[] Serialize<T>(T data);
         string SerializeToString<T>(T data);
         T Deserialize<T>(byte[] data);
         object Deserialize(byte[] data, Type type);
         void PopulateObject(byte[] data, object target);
         void PopulateObject(object source, object target);
    }
}