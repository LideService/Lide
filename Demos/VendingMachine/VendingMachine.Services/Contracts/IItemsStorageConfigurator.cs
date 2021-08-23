using VendingMachine.Services.Model;

namespace VendingMachine.Services.Contracts
{
    public interface IItemsStorageConfigurator
    {
        IItemsStorageConfigurator ConfigureItem(VendingItem vendingItem, int? quantity = null);
        IItemsStorageConfigurator RemoveItem(VendingItem vendingItem);
        IItemsStorageConfigurator ChangeItemQuantity(VendingItem vendingItem, int quantity);
        IItemsStorage CloseItemStorage();
    }
}
