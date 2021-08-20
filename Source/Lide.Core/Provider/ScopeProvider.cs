using System;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class ScopeProvider : IScopeProvider
    {
        private string _scopeId;

        public ScopeProvider(IDateTimeFacade dateTimeFacade, IRandomFacade randomFacade)
        {
            var epoch = dateTimeFacade.GetUnixEpoch();
            var ticks = epoch.Ticks;
            var rLong = randomFacade.NextLong();
            _scopeId = Convert.ToBase64String(BitConverter.GetBytes(ticks ^ rLong));
        }

        public void SetPreviousScopes(string previousScopeId)
        {
            _scopeId = $"{previousScopeId}-{_scopeId}";
        }

        public string GetScopeId() => _scopeId;
    }
}