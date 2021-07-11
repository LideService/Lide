namespace Lide.TracingProxy.DataProcessors.Contract
{
    public interface IScopeTracker
    {
        string GetScopeGuid();
        void SetGuidOverride(string newGuid);
    }
}