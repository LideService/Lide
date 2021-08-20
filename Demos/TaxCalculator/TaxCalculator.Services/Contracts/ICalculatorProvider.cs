namespace TaxCalculator.Services.Contracts
{
    public interface ICalculatorProvider
    {
        string AddCalculator(string name, ICalculator calculator, bool overrideCalculator = false);
        ICalculator GetCalculator(string name);
    }
}