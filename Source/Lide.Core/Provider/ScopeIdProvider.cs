using System;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class ScopeIdProvider : IScopeIdProvider
    {
        private string _scopeId;

        public ScopeIdProvider(IDateTimeFacade dateTimeFacade, IRandomFacade randomFacade)
        {
            var epoch = dateTimeFacade.GetUnixEpoch();
            var ticks = epoch.Ticks;
            var rLong = randomFacade.NextLong();
            _scopeId = Convert.ToBase64String(BitConverter.GetBytes(ticks ^ rLong))
                .Replace("+", "")
                .Replace("=", "")
                .Replace("/", "");
        }

        public void SetPreviousScopes(string previousScopeId)
        {
            if (!string.IsNullOrEmpty(previousScopeId))
            {
                _scopeId = $"{previousScopeId}-{_scopeId}";
            }
        }

        public string GetScopeId() => _scopeId;
    }
}