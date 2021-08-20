using System;
using Lide.Core.Contract.Facade;

namespace Lide.Core.Facade
{
    public class DateTimeFacade : IDateTimeFacade
    {
        public DateTime GetUnixEpoch()
        {
            return DateTime.UnixEpoch;
        }

        public DateTime GetDateNow()
        {
            return DateTime.Now;
        }
    }
}