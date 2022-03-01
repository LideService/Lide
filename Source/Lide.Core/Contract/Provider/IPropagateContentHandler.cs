using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Lide.Core.Contract.Provider
{
    public delegate void PropagateContentPrepareHandler(ConcurrentDictionary<string, byte[]> container, string requestPath);
    public delegate void PropagateContentParseHandler(Dictionary<string, byte[]> content, Exception exception, string requestPath);
    public interface IPropagateContentHandler
    {
        event PropagateContentPrepareHandler PrepareForChild;
        event PropagateContentParseHandler ParseFromChild;
        event PropagateContentPrepareHandler PrepareForParent;

        Dictionary<string, byte[]> ParentData { get; set; }
        Dictionary<string, byte[]> GetDataForParent();

        Dictionary<string, byte[]> GetDataForChild(string requestPath);
        void ParseDataFromChild(Dictionary<string, byte[]> content, Exception exception, string requestPath);
    }
}
