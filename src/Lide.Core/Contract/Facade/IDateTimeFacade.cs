using System;

namespace Lide.Core.Contract.Facade;

public interface IDateTimeFacade
{
    DateTime GetUnixEpoch();
    DateTime GetDateNow();
}