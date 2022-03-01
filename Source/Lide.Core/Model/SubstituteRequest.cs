namespace Lide.Core.Model
{
    public class SubstituteRequest
    {
        public string Path { get; set; }
        public byte[] OriginalContent { get; set; }
        public byte[] OriginalHeaders { get; set; }
    }
}