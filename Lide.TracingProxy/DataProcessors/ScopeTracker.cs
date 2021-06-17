using System;
using Lide.TracingProxy.Contracts;

namespace Lide.TracingProxy.DataProcessors
{
    public class ScopeTracker : IScopeTracker
    {
        private string _scopeGuid;

        public ScopeTracker()
        {
            _scopeGuid = Guid.NewGuid().ToString();
        }

        public void SetGuidOverride(string newGuid)
        {
            _scopeGuid = newGuid;
        }

        public string GetScopeGuid()
        {
            return _scopeGuid;
        }
    }
}