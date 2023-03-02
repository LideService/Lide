using System.Collections.Generic;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Services.Contracts
{
    public interface ICalculator
    {
        decimal CalculateAfterTax(decimal grossAmount, List<TaxLevel> appliedTaxes);
    }
}
