using System.Collections.Generic;
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
            var calculator = GetDefaultCalculator();
            var grossAmount = 800m;
            var expectedAmount = 800m;

            var netAmount = calculator.CalculateAfterTax(grossAmount);

            Assert.AreEqual(expectedAmount, netAmount);
        }

        [TestMethod]
        public void When_GrossAmount_Is_AboveBasicLevel_That_OnlyBasicTaxIsApplied()
        {
            var calculator = GetDefaultCalculator();
            var grossAmount = 1400;
            var expectedAmount = 1350;

            var netAmount = calculator.CalculateAfterTax(grossAmount);

            Assert.AreEqual(expectedAmount, netAmount);
        }

        [TestMethod]
        public void When_GrossAmount_Is_AboveAllLevels_That_AllTaxesWillBeApplied()
        {
            var calculator = GetDefaultCalculator();
            var grossAmount = 2400;
            var expectedAmount = 2115;

            var netAmount = calculator.CalculateAfterTax(grossAmount);

            Assert.AreEqual(expectedAmount, netAmount);
        }

        [TestMethod]
        public void When_GrossAmount_Is_AboveUpperLimit_That_TaxIsAppliedToTheLimit()
        {
            var calculator = GetDefaultCalculator();
            var grossAmount = 4000;
            var expectedAmount = 3465;

            var netAmount = calculator.CalculateAfterTax(grossAmount);

            Assert.AreEqual(expectedAmount, netAmount);
        }

        private static ICalculator GetDefaultCalculator()
        {
            var calculator = new Calculator();
            calculator.AddTaxLevels(new List<TaxLevel>
            {
                new TaxLevel("Basic", 900, null, 10),
                new TaxLevel("Social", 1500, 3000, 15),
            });

            return calculator;
        }
    }
}
