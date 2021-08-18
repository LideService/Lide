namespace Lide.Core.Contract.Provider
{
    public interface IMethodParamsSerializer
    {
        string Serialize(object[] methodParams);
        object[] Deserialize(string serialized);
    }
}