namespace Lide.Core.Contract.Provider
{
    public interface IParametersSerializer
    {
        string Serialize(object[] methodParams);
        object[] Deserialize(string serialized);
    }
}