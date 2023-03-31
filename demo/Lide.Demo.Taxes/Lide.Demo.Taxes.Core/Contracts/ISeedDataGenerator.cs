using Lide.Demo.Taxes.Model;

namespace Lide.Demo.Taxes.Core.Contracts;

public interface ISeedDataGenerator
{
    TaxLevel[] GetRandomTaxLevels();
    TaxLevel[] GetPredefinedTaxLevels();
}