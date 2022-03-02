namespace Lide.Core.Model
{
    public class SubstituteMethodBefore
    {
        public long CallId { get; init; }
        public string MethodSignature { get; init; }
        public byte[] InputParameters { get; init; }
    }
}