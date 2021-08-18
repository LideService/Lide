using System;

namespace Lide.Core.Contract.Facade
{
    public interface ISerializerFacade
    {
         string Serialize<T>(T data);
         T Deserialize<T>(string data);
         object Deserialize(string data, Type type);
    }
}