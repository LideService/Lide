namespace Lide.Demo.Exchange.Core.Contracts;

public interface IRandomFacade
{
    decimal GetRandomDecimal(decimal v1, decimal v2);
    int NextInt(int minValue, int maxValue);
    string NextString();
}