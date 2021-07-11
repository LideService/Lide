using Lide.TracingProxy.DataProcessors.Contract;

namespace Lide.TracingProxy.DataProcessors
{
    public class TrackerDummy : IScopeTracker
    {
        public static IScopeTracker Singleton = new TrackerDummy();
        public void SetGuidOverride(string newGuid)
        {
        }

        public string GetScopeGuid()
        {
            return string.Empty;
        }
    }
}