using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Services.Contracts;

namespace TaxCalculator.WebApi.Controllers
{
    [ApiController]
    public class CalculatorController
    {
        private readonly ITaxLevelsState _taxLevelsState;
        private readonly ICalculator _calculator;

        public CalculatorController(
            ITaxLevelsState taxLevelsState,
            ICalculator calculator)
        {
            _taxLevelsState = taxLevelsState;
            _calculator = calculator;
        }

        [HttpPost]
        [Route("api/calculate")]
        public decimal Calculate(InputAmount input)
        {
            var taxes = _taxLevelsState.GetTaxes();
            var result = _calculator.CalculateAfterTax(input.Amount, taxes);
            return result;
        }
    }

    public class InputAmount
    {
        public decimal Amount { get; set; }
    }
}