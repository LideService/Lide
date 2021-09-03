namespace Lide.Core.Model
{
    public class SubstituteBefore
    {
        public int Id { get; set; }
        public string MethodSignature { get; set; }
        public string CallerSignature { get; set; }
        public byte[] InputParameters { get; set; }
    }
}