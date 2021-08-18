using System;
using Lide.Core.Contract.Facade;

namespace Lide.Core.Facade
{
    public class ConsoleFacade : IConsoleFacade
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