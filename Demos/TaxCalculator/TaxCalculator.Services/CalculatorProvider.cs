using System.Collections.Generic;
using TaxCalculator.Services.Contracts;

namespace TaxCalculator.Services
{
    public class CalculatorProvider : ICalculatorProvider
    {
        private readonly Dictionary<string, ICalculator> _calculators;

        public CalculatorProvider()
        {
            _calculators = new Dictionary<string, ICalculator>();
        }

        public string AddCalculator(string name, ICalculator calculator, bool overrideCalculator)
        {
            if (_calculators.ContainsKey(name))
            {
                if (overrideCalculator)
                {
                    _calculators[name] = calculator;
                    return $"Calculator {name} overriden";
                }

                return $"Calculator {name} already exists";
            }

            _calculators[name] = calculator;
            return $"Calculator {name} added";
        }

        public ICalculator GetCalculator(string name)
        {
            return _calculators.ContainsKey(name) ? _calculators[name] : null;
        }
    }
}