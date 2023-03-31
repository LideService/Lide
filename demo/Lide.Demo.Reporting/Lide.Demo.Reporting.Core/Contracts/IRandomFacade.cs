namespace Lide.Demo.Reporting.Core.Contracts;

public interface IRandomFacade
{
    int NextInt(int minValue, int maxValue);
    string NextString();
}