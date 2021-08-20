using System;
using System.Collections.Generic;
using TaxCalculator.Services;
using TaxCalculator.Services.Contracts;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Exe
{
    public class AskToCalculate
    {
        private readonly ICalculator _calculator;

        public AskToCalculate()
        {
            _calculator = CreateDefaultCalculator();
        }

        public void StartAsking()
        {
            Console.WriteLine(_calculator.GetInfo());
            while (true)
            {
                Console.WriteLine("Enter amount (or leave empty to exit):");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("See you again!");
                    break;
                }

                var validInput = decimal.TryParse(input, out var grossAmount);
                if (!validInput)
                {
                    Console.WriteLine($"Entered value of '{input}' is not a valid number");
                    continue;
                }

                var netAmount = _calculator.CalculateAfterTax(grossAmount);
                Console.WriteLine($"Net amount after taxes is {netAmount}");
            }
        }

        private static ICalculator CreateDefaultCalculator()
        {
            var calculator = new Calculator();
            calculator.AddTaxLevels(new List<TaxLevel>
            {
                new TaxLevel("Basic", 1000, null, 10),
                new TaxLevel("Social", 1000, 3000, 15),
            });

            return calculator;
        }
    }
}
