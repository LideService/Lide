using System.Collections.Generic;
using Lide.Demo.Reporting.Model;

namespace Lide.Demo.Reporting.Core.Contracts;

public interface ITaxLevelsState
{
    void AddTaxLevel(TaxLevel taxLevel);
    void AddTaxLevels(TaxLevel[] taxLevel);
    TaxLevel GetTaxByName(string name);
    TaxLevel[] GetTaxesByName(TaxName[] names);
    TaxLevel[] GetTaxes();
}
