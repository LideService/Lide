namespace Lide.Core.Model
{
    public class SubstituteOutgoingRequest
    {
        public string Path { get; set; }
        public long RequestId { get; set; }
        public byte[] Content { get; set; }
    }
}