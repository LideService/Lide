using Lide.Demo.Reporting.Core.Service;
using Lide.Demo.Reporting.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Demo.Reporting.Test;

[TestClass]
public class TestCalculator
{
    [TestMethod]
    public void When_GrossAmount_Is_BelowAllTaxLevels_That_NoTaxesWillBeApplied()
    {
        var taxes = GetDefaultTaxes();
        var calculator = new Calculator();
        var grossAmount = 800m;
        var expectedAmount = 800m;

        var netAmount = calculator.CalculateAfterTax(grossAmount, taxes);

        Assert.AreEqual(expectedAmount, netAmount);
    }

    [TestMethod]
    public void When_GrossAmount_Is_AboveBasicLevel_That_OnlyBasicTaxIsApplied()
    {
        var taxes = GetDefaultTaxes();
        var calculator = new Calculator();
        var grossAmount = 1400;
        var expectedAmount = 1350;

        var netAmount = calculator.CalculateAfterTax(grossAmount, taxes);

        Assert.AreEqual(expectedAmount, netAmount);
    }

    [TestMethod]
    public void When_GrossAmount_Is_AboveAllLevels_That_AllTaxesWillBeApplied()
    {
        var taxes = GetDefaultTaxes();
        var calculator = new Calculator();
        var grossAmount = 2400;
        var expectedAmount = 2115;

        var netAmount = calculator.CalculateAfterTax(grossAmount, taxes);

        Assert.AreEqual(expectedAmount, netAmount);
    }

    [TestMethod]
    public void When_GrossAmount_Is_AboveUpperLimit_That_TaxIsAppliedToTheLimit()
    {
        var taxes = GetDefaultTaxes();
        var calculator = new Calculator();
        var grossAmount = 4000;
        var expectedAmount = 3465;

        var netAmount = calculator.CalculateAfterTax(grossAmount, taxes);

        Assert.AreEqual(expectedAmount, netAmount);
    }

    private static TaxLevel[] GetDefaultTaxes()
    {
        return new[]
        {
            new TaxLevel("Basic", 900, null, 10),
            new TaxLevel("Social", 1500, 3000, 15),
        };
    }
}