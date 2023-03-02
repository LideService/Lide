using System;
using TaxCalculator.Services.Contracts;

namespace TaxCalculator.Services
{
    public class DateTimeFacade : IDateTimeFacade
    {
        public DateTime GetDatetimeNow()
        {
            return DateTime.Now;
        }
    }
}