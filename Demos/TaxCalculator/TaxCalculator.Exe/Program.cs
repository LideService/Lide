using System;
using System.Collections.Generic;
using System.Text.Json;
using TaxCalculator.Services.Model;

namespace TaxCalculator.Exe
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var levels = new List<TaxLevel>()
            {
                new TaxLevel("1", 0, 200, 3),
                new TaxLevel("2", 300, 500, 7),
            };

            Console.WriteLine(JsonSerializer.Serialize(levels));
            return;

            // var askToCalculate = new AskToCalculate();
            // askToCalculate.StartAsking();
        }
    }
}
