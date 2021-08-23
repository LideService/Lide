using System;
using VendingMachine.Services.Contracts;

namespace VendingMachine.Services.Facades
{
    public class ConsoleFacade : IConsoleFacade
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
