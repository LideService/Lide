namespace Lide.Core.Model
{
    public class SubstituteBefore
    {
        public long CallId { get; init; }
        public string MethodSignature { get; init; }
        public byte[] InputParameters { get; init; }
    }
}