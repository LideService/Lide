using System.Collections.Generic;
using Lide.Demo.Taxes.Model;

namespace Lide.Demo.Taxes.Core.Contracts;

public interface ITaxLevelsState
{
    void AddTaxLevel(TaxLevel taxLevel);
    void AddTaxLevels(TaxLevel[] taxLevel);
    TaxLevel GetTaxByName(string name);
    TaxLevel[] GetTaxesByName(TaxName[] names);
    TaxLevel[] GetTaxes();
}
