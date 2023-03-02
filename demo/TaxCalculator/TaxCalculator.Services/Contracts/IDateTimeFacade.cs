using System;

namespace TaxCalculator.Services.Contracts
{
    public interface IDateTimeFacade
    {
        DateTime GetDatetimeNow();
    }
}