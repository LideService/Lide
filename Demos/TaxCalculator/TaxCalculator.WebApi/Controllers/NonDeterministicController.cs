using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Services.Contracts;
using TaxCalculator.Services.Model;

namespace TaxCalculator.WebApi.Controllers
{
    [ApiController]
    public class NonDeterministicController
    {
        private readonly INonDeterministic1 _nonDeterministic1;

        public NonDeterministicController(INonDeterministic1 nonDeterministic1)
        {
            _nonDeterministic1 = nonDeterministic1;
        }
        
        [HttpPost]
        [Route("nond/noisolation")]
        public BadDesignData InvokeNoIsolation(decimal initialValue)
        {
            var result = _nonDeterministic1.InitializeAndDoStuff_NoIsolation(initialValue);
            return result;
        }
        
        [HttpPost]
        [Route("nond/withisolation")]
        public BadDesignData InvokeWithIsolation(decimal initialValue)
        {
            var result = _nonDeterministic1.InitializeAndDoStuff_WithIsolation(initialValue);
            return result;
        }
    }
}