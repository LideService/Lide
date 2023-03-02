using System;
using Lide.Core.Contract.Facade;

namespace Lide.Core.Facade;

public class LoggerFacade : ILoggerFacade
{
    public void Log(string message)
    {
        Console.WriteLine($"[Lide.Log] {message}");
    }

    public void LogError(string message)
    {
        Console.WriteLine($"[Lide.Error]: {message}");
    }
}