using System.Collections.Generic;
using VendingMachine.Services.Model;

namespace VendingMachine.Services.Contracts
{
    public interface IIOProcessor
    {
        bool ParseInputToItemId(string inputItem, out int itemId);
        bool ParseInputToPence(string inputCoins, out int coinWorthInPence);

        void WriteChange(List<int> changeCoins);
        void WriteClientTotalAmount(int clientAmount);
        void WriteStorageContent(Dictionary<VendingItem, int> storageContnent);
        void WriteInvalidCointWorth(int coinWorth);
        void WriteCannotAcceptClientCoin(int coinWorth);
        void WriteInvalidInput(string input);
        void WriteInvalidItem(int itemId);
        void WriteNotEnoughChange();
        void WriteNotEnoughClientAmount(int costInPence);
        void WriteAcceptableCoins(List<int> list);
        void WriteNotEnoughQuantiy(int itemId);
        void WriteItemOutput(VendingItem vendingItem);
    }
}
