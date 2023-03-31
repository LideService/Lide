using Lide.Demo.Taxes.Model;

namespace Lide.Demo.Taxes.Core.Contracts;

public interface ICalculator
{
    decimal CalculateAfterTax(decimal grossAmount, TaxLevel[] appliedTaxes);
}
