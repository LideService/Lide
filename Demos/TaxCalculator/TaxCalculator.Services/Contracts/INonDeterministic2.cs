using TaxCalculator.Services.Model;

namespace TaxCalculator.Services.Contracts
{
    public interface INonDeterministic2
    {
        void Volatile_TreatInputAsNonMutable_NoIsolation(BadDesignData data);
        void Volatile_TreatInputAsNonMutable_WithIsolation(BadDesignData data);
    }
}