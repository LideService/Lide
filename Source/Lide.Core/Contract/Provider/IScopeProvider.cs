namespace Lide.Core.Contract.Provider
{
    public interface IScopeProvider
    {
        public void SetPreviousScopes(string scopeId);
        string GetScopeId();
    }
}