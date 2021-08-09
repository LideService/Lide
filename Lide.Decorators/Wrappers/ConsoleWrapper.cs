using System;
using Lide.Decorators.Contract;

namespace Lide.Decorators.Wrappers
{
    public class ConsoleWrapper : IConsoleWrapper
    {
        public void Write(string message)
        {
            Console.Write(message);
        }
        
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}