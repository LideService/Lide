using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxCalculator.Services;
using TaxCalculator.Services.Contracts;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Test
{
    [TestClass]
    public class TaxCalculatorTest
    {
        [TestMethod]
        public void When_GrossAmount_Is_BelowAllTaxLevels_That_NoTaxesWillBeApplied()
        {
            var taxes = GetDefaultTaxes().GetTaxes();
            var calculator = new Calculator();
            var grossAmount = 800m;
            var expectedAmount = 800m;

            var netAmount = calculator.CalculateAfterTax(grossAmount, taxes);

            Assert.AreEqual(expectedAmount, netAmount);
        }

        [TestMethod]
        public void When_GrossAmount_Is_AboveBasicLevel_That_OnlyBasicTaxIsApplied()
        {
            var taxes = GetDefaultTaxes().GetTaxes();
            var calculator = new Calculator();
            var grossAmount = 1400;
            var expectedAmount = 1350;

            var netAmount = calculator.CalculateAfterTax(grossAmount, taxes);

            Assert.AreEqual(expectedAmount, netAmount);
        }

        [TestMethod]
        public void When_GrossAmount_Is_AboveAllLevels_That_AllTaxesWillBeApplied()
        {
            var taxes = GetDefaultTaxes().GetTaxes();
            var calculator = new Calculator();
            var grossAmount = 2400;
            var expectedAmount = 2115;

            var netAmount = calculator.CalculateAfterTax(grossAmount, taxes);

            Assert.AreEqual(expectedAmount, netAmount);
        }

        [TestMethod]
        public void When_GrossAmount_Is_AboveUpperLimit_That_TaxIsAppliedToTheLimit()
        {
            var taxes = GetDefaultTaxes().GetTaxes();
            var calculator = new Calculator();
            var grossAmount = 4000;
            var expectedAmount = 3465;

            var netAmount = calculator.CalculateAfterTax(grossAmount, taxes);

            Assert.AreEqual(expectedAmount, netAmount);
        }

        private static ITaxLevelsState GetDefaultTaxes()
        {
            var taxes = new TaxLevelsState();
            taxes.AddTaxLevel(new TaxLevel("Basic", 900, null, 10));
            taxes.AddTaxLevel(new TaxLevel("Social", 1500, 3000, 15));

            return taxes;
        }
    }
}
