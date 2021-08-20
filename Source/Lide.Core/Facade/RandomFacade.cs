using System;
using Lide.Core.Contract.Facade;

namespace Lide.Core.Facade
{
    public class RandomFacade : IRandomFacade
    {
        private readonly Random _random;

        public RandomFacade()
        {
            _random = new Random();
        }

        public long NextLong()
        {
            var buffer = new byte[8];
            _random.NextBytes(buffer);
            return Math.Abs(BitConverter.ToInt64(buffer, 0));
        }
    }
}