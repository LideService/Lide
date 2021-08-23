using System.Collections.Generic;

namespace VendingMachine.Services.Contracts
{
    public interface ICoinsStorageConfigurator
    {
        ICoinsStorageConfigurator SupplyWithCoins(int coinWorth, int quantity = 1);
        ICoinsStorageConfigurator ExtractCoins(int coinWorth, int quantity = 1);
        ICoinsStorageConfigurator AddAcceptableCoinWorth(int cointWorth);
        ICoinsStorageConfigurator RemoveAcceptableCoinWorth(int cointWorth);
        ICoinsStorage CloseCoinsVault();
        List<int> ShowCurrentCoins();
        int ShowTotalAmountInTheVault();
    }
}
