using Lide.Demo.Reporting.Model;

namespace Lide.Demo.Reporting.Core.Contracts;

public interface ICalculator
{
    decimal CalculateAfterTax(decimal grossAmount, TaxLevel[] appliedTaxes);
}
