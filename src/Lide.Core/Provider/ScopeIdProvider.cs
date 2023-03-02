using System;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider;

public class ScopeIdProvider : IScopeIdProvider
{
    private readonly string _scopeId;
    private string _rootScopeId;

    public ScopeIdProvider(IDateTimeFacade dateTimeFacade, IRandomFacade randomFacade)
    {
        _rootScopeId = string.Empty;
        var epoch = dateTimeFacade.GetUnixEpoch();
        var ticks = epoch.Ticks;
        var rLong = randomFacade.NextLong();
        _scopeId = Convert.ToBase64String(BitConverter.GetBytes(ticks ^ rLong))
            .Replace("+", "")
            .Replace("=", "")
            .Replace("/", "");
    }

    public void SetRootScopeId(string rootScopeId)
    {
        _rootScopeId = rootScopeId ?? _scopeId;
    }

    public string GetRootScopeId() => _rootScopeId;
    public string GetCurrentScopeId() => _scopeId;
}