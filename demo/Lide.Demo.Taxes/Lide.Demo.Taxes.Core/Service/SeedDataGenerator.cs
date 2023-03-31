using Lide.Demo.Taxes.Core.Contracts;
using Lide.Demo.Taxes.Model;

namespace Lide.Demo.Taxes.Core.Service;

public class SeedDataGenerator : ISeedDataGenerator
{
    private readonly IRandomFacade _randomFacade;

    public SeedDataGenerator(IRandomFacade randomFacade)
    {
        _randomFacade = randomFacade;
    }

    public TaxLevel[] GetRandomTaxLevels()
    {
        var count = _randomFacade.NextInt(5, 20);
        var taxLevels = new TaxLevel[count];
        for (var i = 0; i < count; i++)
        {
            var name = _randomFacade.NextString();
            var min = _randomFacade.NextInt(0, 10000);
            int? max = _randomFacade.NextInt(min, 10000);
            if (_randomFacade.NextInt(0, 10) == 0)
            {
                max = null;
            }

            var rate = _randomFacade.NextInt(0, 100);
            taxLevels[i] = new TaxLevel(name, min, max, rate);
        }

        return taxLevels;
    }

    public TaxLevel[] GetPredefinedTaxLevels()
    {
        return new[]
        {
            new TaxLevel("NotTaxable", 0, 1000, 0m),
            new TaxLevel("MinorTax", 1000, 2200, 2),
            new TaxLevel("Over1000", 1000, null, 5),
            new TaxLevel("Communism", 2200, null, 49),
        };
    }
}
