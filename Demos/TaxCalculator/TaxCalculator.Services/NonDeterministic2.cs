using System;
using TaxCalculator.Services.Contracts;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Services
{
    public class NonDeterministic2 : INonDeterministic2
    {
        private readonly IDateTimeFacade _dateTimeFacade;

        public NonDeterministic2(IDateTimeFacade dateTimeFacade)
        {
            _dateTimeFacade = dateTimeFacade;
        }

        public void Volatile_TreatInputAsNonMutable_NoIsolation(BadDesignData data)
        {
            var dateTime = DateTime.Now;
            data.Field1 += dateTime.Second % 6;
            data.Field2 += dateTime.Second % 9;
        }

        public void Volatile_TreatInputAsNonMutable_WithIsolation(BadDesignData data)
        {
            var dateTime = _dateTimeFacade.GetDatetimeNow();
            data.Field1 += dateTime.Second % 6;
            data.Field2 += dateTime.Second % 9;
        }
    }
}