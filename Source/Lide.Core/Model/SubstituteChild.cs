using System;

namespace Lide.Core.Model
{
    public class SubstituteChild
    {
        public string Path { get; set; }
        public byte[] Content { get; set; }
        public Exception Exception { get; set; }
    }
}