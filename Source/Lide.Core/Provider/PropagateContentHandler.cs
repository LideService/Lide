using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class PropagateContentHandler : IPropagateContentHandler
    {
        public event PropagateContentPrepareHandler PrepareForChild;
        public event PropagateContentParseHandler ParseFromChild;
        public event PropagateContentPrepareHandler PrepareForParent;

        public Dictionary<string, byte[]> ParentData { get; set; } = new ();
        public Dictionary<string, byte[]> GetDataForParent()
        {
            var container = new ConcurrentDictionary<string, byte[]>();
            PrepareForParent?.Invoke(container, string.Empty);
            return new Dictionary<string, byte[]>(container);
        }

        public Dictionary<string, byte[]> GetDataForChild(string requestPath)
        {
            var container = new ConcurrentDictionary<string, byte[]>();
            PrepareForChild?.Invoke(container, requestPath);
            return new Dictionary<string, byte[]>(container);
        }

        public void ParseDataFromChild(Dictionary<string, byte[]> content, Exception exception, string requestPath)
        {
            ParseFromChild?.Invoke(content, exception, requestPath);
        }
    }
}