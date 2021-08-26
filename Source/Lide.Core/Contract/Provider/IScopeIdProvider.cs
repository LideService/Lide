namespace Lide.Core.Contract.Provider
{
    public interface IScopeIdProvider
    {
        public void SetPreviousScopes(string scopeId);
        string GetScopeId();
    }
}