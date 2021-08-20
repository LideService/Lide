using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Services.Contracts;

namespace TaxCalculator.WebApi.Controllers
{
    [ApiController]
    public class CalculatorController
    {
        private readonly ICalculatorProvider _calculatorProvider;

        public CalculatorController(ICalculatorProvider calculatorProvider)
        {
            _calculatorProvider = calculatorProvider;
        }

        [HttpGet]
        [Route("api/calculate")]
        public decimal Calculate(string name, decimal amount)
        {
            var calculator = _calculatorProvider.GetCalculator(name);
            return calculator?.CalculateAfterTax(amount) ?? -1;
        }

        [HttpGet]
        [Route("api/info")]
        public string GetInfo(string name)
        {
            var calculator = _calculatorProvider.GetCalculator(name);
            return calculator?.GetInfo() ?? "Missing calculator!";
        }
    }
}