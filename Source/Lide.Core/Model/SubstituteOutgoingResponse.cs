using System;

namespace Lide.Core.Model
{
    public class SubstituteOutgoingResponse
    {
        public long RequestId { get; set; }
        public string Path { get; set; }
        public byte[] Content { get; set; }
        public Exception Exception { get; set; }
    }
}