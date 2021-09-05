using System.Collections.Generic;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Services.Contracts
{
    public interface ITaxLevelsState
    {
        void AddTaxLevel(TaxLevel taxLevel);
        void RemoveTaxLevel(string name);
        List<TaxLevel> GetTaxes();
        string GetInfo();
    }
}
