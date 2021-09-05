using TaxCalculator.Services.Model;

namespace TaxCalculator.Services.Contracts
{
    public interface INonDeterministic1
    {
        BadDesignData InitializeAndDoStuff_NoIsolation(decimal input);
        BadDesignData InitializeAndDoStuff_WithIsolation(decimal input);
    }
}