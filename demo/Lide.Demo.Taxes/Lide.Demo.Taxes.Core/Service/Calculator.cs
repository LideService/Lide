﻿using Lide.Demo.Taxes.Core.Contracts;
using Lide.Demo.Taxes.Model;

namespace Lide.Demo.Taxes.Core.Service;

public class Calculator : ICalculator
{
    public decimal CalculateAfterTax(decimal grossAmount, TaxLevel[] appliedTaxes)
    {
        var totalTax = 0m;

        foreach (var taxLevel in appliedTaxes)
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