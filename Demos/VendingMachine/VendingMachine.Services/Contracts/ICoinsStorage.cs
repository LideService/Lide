using System.Collections.Generic;

namespace VendingMachine.Services.Contracts
{
    public interface ICoinsStorage
    {
        ICoinsStorageConfigurator OpenCoinsVault();

        bool AddClientCoin(int coinWorth);
        bool IsClientAmountEnough(int requestedAmount);
        bool CheckForChange(int requestedAmount);
        bool CompleteRequest(int requestedAmount);

        List<int> GetChange();
        List<int> GetAcceptableCoins();
        bool ValidateCoinWorth(int coinsWorth);
        int GetCurrentClientAmount();
    }
}
