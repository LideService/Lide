using System.Collections.Generic;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Services.Contracts
{
    public interface ICalculator
    {
        ICalculator AddTaxLevels(IList<TaxLevel> taxLevels);

        decimal CalculateAfterTax(decimal grossAmount);
        double CalculateAfterTax(double grossAmount);
        int CalculateAfterTax(int grossAmount);
        string GetInfo();
    }
}
