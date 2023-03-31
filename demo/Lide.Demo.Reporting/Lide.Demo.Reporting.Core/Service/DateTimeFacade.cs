using System;
using Lide.Demo.Reporting.Core.Contracts;

namespace Lide.Demo.Reporting.Core.Service;

public class DateTimeFacade : IDateTimeFacade
{
    public DateTime GetDateTimeNow()
    {
        return DateTime.Now;
    }
}