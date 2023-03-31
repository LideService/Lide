using System;
using Lide.Demo.Exchange.Core.Contracts;

namespace Lide.Demo.Exchange.Core.Service;

public class DateTimeFacade : IDateTimeFacade
{
    public DateTime GetDateTimeNow()
    {
        return DateTime.Now;
    }
}