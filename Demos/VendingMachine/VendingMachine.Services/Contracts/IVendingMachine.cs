namespace VendingMachine.Services.Contracts
{
    public interface IVendingMachine
    {
        IVendingMachineConfigurator StopVendingMachine();
        void ProcessInput(string input);
        void PrintStartup();
    }
}
