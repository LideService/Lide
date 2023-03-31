using Lide.Demo.Reporting.Model;

namespace Lide.Demo.Reporting.Core.Contracts;

public interface ISeedDataGenerator
{
    TaxLevel[] GetRandomTaxLevels();
    TaxLevel[] GetPredefinedTaxLevels();
}