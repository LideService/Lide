using System;
using System.Collections.Generic;
using System.Linq;
using TaxCalculator.Services.Contracts;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Services
{
    public class Calculator : ICalculator
    {
        private readonly IList<TaxLevel> _appliedTaxes;

        public Calculator()
        {
            _appliedTaxes = new List<TaxLevel>();
        }

        public ICalculator AddTaxLevels(IList<TaxLevel> taxLevels)
        {
            foreach (var taxLevel in taxLevels)
            {
                _appliedTaxes.Add(taxLevel);
            }

            return this;
        }

        public string GetInfo()
        {
            var taxesInfo = string.Join(Environment.NewLine, _appliedTaxes.Select(x => x.GetInfo()));
            return $"Calculator is configured with the following taxes:{Environment.NewLine}{taxesInfo}";
        }

        public decimal CalculateAfterTax(decimal grossAmount)
        {
            return Calculate(grossAmount);
        }

        public double CalculateAfterTax(double grossAmount)
        {
            return (double)Calculate((decimal)grossAmount);
        }

        public int CalculateAfterTax(int grossAmount)
        {
            return (int)Calculate(grossAmount);
        }

        private decimal Calculate(decimal grossAmount)
        {
            var totalTax = 0m;

            foreach (var taxLevel in _appliedTaxes)
            {
                var taxableAmount = grossAmount;
                if (taxLevel.LowerLimit.HasValue && grossAmount > taxLevel.LowerLimit.Value)
                {
                    taxableAmount -= taxLevel.LowerLimit.Value;
                }
                else
                {
                    taxableAmount = 0;
                }

                if (taxLevel.UpperLimit.HasValue && grossAmount > taxLevel.UpperLimit.Value)
                {
                    taxableAmount -= grossAmount - taxLevel.UpperLimit.Value;
                }

                totalTax += taxableAmount * (taxLevel.Percentage / 100m);
            }

            return grossAmount - totalTax;
        }
    }
}
