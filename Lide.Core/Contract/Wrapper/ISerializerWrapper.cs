using System;

namespace Lide.Core.Contract.Wrapper
{
    public interface ISerializerWrapper
    {
         string Serialize<T>(T data);
         T Deserialize<T>(string data);
         object Deserialize(string data, Type type);
    }
}