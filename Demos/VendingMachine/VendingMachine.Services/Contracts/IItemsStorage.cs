using System.Collections.Generic;
using VendingMachine.Services.Model;

namespace VendingMachine.Services.Contracts
{
    public interface IItemsStorage
    {
        IItemsStorageConfigurator OpenItemStorage();
        Dictionary<VendingItem, int> GetStorageContentWithQuantity();
        bool CheckForItem(int itemId, out VendingItem vendingItem);
        bool CheckForQuantity(VendingItem vendingItem, int neededQuantity = 1);
        bool GetItemFromStorage(VendingItem vendingItem);
    }
}
