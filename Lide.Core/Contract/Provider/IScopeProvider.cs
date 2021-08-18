namespace Lide.Core.Contract.Provider
{
    public interface IScopeProvider
    {
        void OverrideScope(string scopeId);
        string GetScopeId();
    }
}