namespace Lide.TracingProxy.Contracts
{
    public interface IScopeTracker
    {
        string GetScopeGuid();
        void SetGuidOverride(string newGuid);
    }
}