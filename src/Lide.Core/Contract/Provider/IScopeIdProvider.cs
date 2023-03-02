namespace Lide.Core.Contract.Provider;

public interface IScopeIdProvider
{
    void SetRootScopeId(string rootScopeId);
    string GetRootScopeId();
    string GetCurrentScopeId();
}