namespace Lide.Core.Model.Settings
{
    public class PropagateHeaders
    {
        public bool Enabled { get; set; }
        public int Depth { get; set; }
        public int NextDepth => Depth + 1;
    }
}