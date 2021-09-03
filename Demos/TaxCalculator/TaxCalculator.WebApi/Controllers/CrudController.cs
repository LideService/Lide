using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Services.Contracts;
using TaxCalculator.WebApi.Model;

namespace TaxCalculator.WebApi.Controllers
{
    [ApiController]
    public class CrudController : Controller
    {
        private readonly ICalculatorProvider _calculatorProvider;

        public CrudController(ICalculatorProvider calculatorProvider)
        {
            _calculatorProvider = calculatorProvider;
        }

        [HttpPost]
        [Route("api/create")]
        public string CreateNew(ConfigureCalculator configuration)
        {
            var newCalculator = (ICalculator)HttpContext.RequestServices.GetService(typeof(ICalculator));
            newCalculator!.AddTaxLevels(configuration.TaxLevels);
            var result = _calculatorProvider.AddCalculator(configuration.Name, newCalculator, configuration.Override);
            return result;
        }
    }
}