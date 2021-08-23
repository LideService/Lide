namespace VendingMachine.Services.Contracts
{
    public interface IVendingMachineConfigurator
    {
        ICoinsStorageConfigurator OpenCoinsStorage();
        IItemsStorageConfigurator OpenItemsStorage();
        IVendingMachine StartVendingMachine();
    }
}
