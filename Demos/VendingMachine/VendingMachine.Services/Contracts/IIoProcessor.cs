using System.Collections.Generic;
using VendingMachine.Services.Model;

namespace VendingMachine.Services.Contracts
{
    public interface IIoProcessor
    {
        bool ParseInputToItemId(string inputItem, out int itemId);
        bool ParseInputToPence(string inputCoins, out int coinWorthInPence);

        void WriteChange(IEnumerable<int> changeCoins);
        void WriteClientTotalAmount(int clientAmount);
        void WriteStorageContent(Dictionary<VendingItem, int> storageContent);
        void WriteInvalidCoinsWorth(int coinWorth);
        void WriteCannotAcceptClientCoin(int coinWorth);
        void WriteInvalidInput(string input);
        void WriteInvalidItem(int itemId);
        void WriteNotEnoughChange();
        void WriteNotEnoughClientAmount(int costInPence);
        void WriteAcceptableCoins(IEnumerable<int> list);
        void WriteNotEnoughQuantity(int itemId);
        void WriteItemOutput(VendingItem vendingItem);
    }
}
