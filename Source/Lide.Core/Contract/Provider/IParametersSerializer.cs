namespace Lide.Core.Contract.Provider
{
    public interface IParametersSerializer
    {
        byte[] Serialize(object[] methodParams);
        object[] Deserialize(byte[] serialized);
        byte[] SerializeSingle(object methodParams);
        object DeserializeSingle(byte[] serialized);
    }
}