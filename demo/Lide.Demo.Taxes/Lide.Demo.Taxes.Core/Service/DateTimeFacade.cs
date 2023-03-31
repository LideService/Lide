using System;
using Lide.Demo.Taxes.Core.Contracts;

namespace Lide.Demo.Taxes.Core.Service;

public class DateTimeFacade : IDateTimeFacade
{
    public DateTime GetDateTimeNow()
    {
        return DateTime.Now;
    }
}