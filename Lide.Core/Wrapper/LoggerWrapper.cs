using System;
using Lide.Core.Contract.Wrapper;

namespace Lide.Core.Wrapper
{
    public class LoggerWrapper : ILoggerWrapper
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
}