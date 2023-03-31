using System;
using System.Collections.Concurrent;
using System.Linq;
using Lide.Demo.Reporting.Core.Contracts;
using Lide.Demo.Reporting.Model;

namespace Lide.Demo.Reporting.Core.Service;

public class TaxLevelsState : ITaxLevelsState
{
    private static readonly ConcurrentBag<TaxLevel> AppliedTaxes = new();

    public void AddTaxLevel(TaxLevel taxLevel)
    {
        AppliedTaxes.Add(taxLevel);
    }

    public void AddTaxLevels(TaxLevel[] taxLevels)
    {
        Array.ForEach(taxLevels, AddTaxLevel);
    }

    public TaxLevel[] GetTaxes()
    {
        return AppliedTaxes.ToArray();
    }

    public TaxLevel GetTaxByName(string name)
    {
        var taxLevel = AppliedTaxes.FirstOrDefault(x => x.Name == name);
        return taxLevel ?? new TaxLevel("", null, null, 0);
    }

    public TaxLevel[] GetTaxesByName(TaxName[] names)
    {
        var taxLevels = names.Select(name => GetTaxByName(name.Value)).ToList();
        return taxLevels.ToArray();
    }
}