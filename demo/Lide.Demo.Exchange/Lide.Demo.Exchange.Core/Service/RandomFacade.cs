using System;
using Lide.Demo.Exchange.Core.Contracts;

namespace Lide.Demo.Exchange.Core.Service;

public class RandomFacade : IRandomFacade
{
    private static readonly string[] Consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
    private static readonly string[] Vowels = { "a", "e", "i", "o", "u", "ae", "y" };

    public decimal GetRandomDecimal(decimal v1, decimal v2)
    {
        return Random.Shared.Next((int)v1 * 1000, (int)v2 * 1000) / 1000;
    }

    public int NextInt(int minValue, int maxValue)
    {
        return Random.Shared.Next(minValue, maxValue);
    }

    public string NextString()
    {
        var result = string.Empty;
        var length = Random.Shared.Next(4, 12);
        result += Consonants[Random.Shared.Next(Consonants.Length)].ToUpper();
        result += Vowels[Random.Shared.Next(Vowels.Length)];
        for (var i = 2; i < length; i++)
        {
            result += Consonants[Random.Shared.Next(Consonants.Length)];
            result += Vowels[Random.Shared.Next(Vowels.Length)];
        }

        return result;
    }
}