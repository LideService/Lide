using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Services.Contracts;
using TaxCalculator.Services.Model;

namespace TaxCalculator.WebApi.Controllers
{
    [ApiController]
    public class TaxLevelsController
    {
        private readonly ITaxLevelsState _taxLevelsState;

        public TaxLevelsController(ITaxLevelsState taxLevelsState)
        {
            _taxLevelsState = taxLevelsState;
        }
        
        [HttpPost]
        [Route("tax/add")]
        public void AddTaxLevel(TaxLevel taxLevel)
        {
            _taxLevelsState.AddTaxLevel(taxLevel);
        }

        [HttpPost]
        [Route("tax/remove")]
        public void Calculate(string name)
        {
            _taxLevelsState.RemoveTaxLevel(name);
        }

        [HttpPost]
        [Route("tax/info")]
        public string GetInfo()
        {
            return _taxLevelsState.GetInfo();
        }
    }
}