namespace Lide.Core.Contract.Provider
{
    public interface IJsonSerializeProvider
    {
        string Serialize(object data);
        T Deserialize<T>(string data);
    }
}