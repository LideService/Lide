using System.Diagnostics.CodeAnalysis;

namespace Lide.Demo.Reporting.Model;

public class TaxLevel
{
    public TaxLevel()
    {
    }

    [SetsRequiredMembers]
    public TaxLevel(string name, decimal? lowerLimit, decimal? upperLimit, decimal percentage)
    {
        Name = name;
        LowerLimit = lowerLimit;
        UpperLimit = upperLimit;
        Percentage = percentage;
    }

    required public string Name { get; set; }
    required public decimal? LowerLimit { get; set; }
    required public decimal? UpperLimit { get; set; }
    required public decimal Percentage { get; set; }
}
