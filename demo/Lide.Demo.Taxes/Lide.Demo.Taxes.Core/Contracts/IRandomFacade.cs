namespace Lide.Demo.Taxes.Core.Contracts;

public interface IRandomFacade
{
    int NextInt(int minValue, int maxValue);
    string NextString();
}