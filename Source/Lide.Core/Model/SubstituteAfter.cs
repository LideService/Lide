namespace Lide.Core.Model
{
    public class SubstituteAfter
    {
        public long CallId { get; init; }
        public bool IsException { get; init; }
        public byte[] OutputData { get; init; }
        public byte[] InputParameters { get; set; }
    }
}