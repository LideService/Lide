namespace Lide.Core.Model
{
    public class SerializedParameter
    {
        public static SerializedParameter Null = new () { TypeName = null, Data = null };
        public string TypeName { get; init; }
        public byte[] Data { get; init; }
    }
}