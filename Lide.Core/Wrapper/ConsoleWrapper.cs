using System;
using Lide.Core.Contract.Wrapper;

namespace Lide.Core.Wrapper
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