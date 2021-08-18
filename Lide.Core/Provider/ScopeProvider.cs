using System;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class ScopeProvider : IScopeProvider
    {
        private string _id;
        public ScopeProvider(IGuidFacade guidFacade)
        {
            _id = guidFacade.NewGuid();
        }

        public void OverrideScope(string scopeId)
        {
            _id = scopeId;
        }

        public string GetScopeId()
        {
            return _id;
        }
    }
}