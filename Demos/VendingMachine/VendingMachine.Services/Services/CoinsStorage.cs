using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Services.Contracts;

namespace VendingMachine.Services.Services
{
    public class CoinsStorage : ICoinsStorageConfigurator, ICoinsStorage
    {
        private static readonly string _invalidCoinWorth = "Invalid coin worth!";
        private static readonly string _invalidCoinQuantity = "Invalid coin quantity!";
        private static readonly string _notEnoughCoins = "Not enough coins to extract!";

        private Dictionary<int, int> _availableCoinsQuantity;
        private Dictionary<int, int> _clientCoinsQuantity;
        private HashSet<int> _acceptableCoinsWorth;

        private CoinsStorage()
        {
            _availableCoinsQuantity = new Dictionary<int, int>();
            _clientCoinsQuantity = new Dictionary<int, int>();
            _acceptableCoinsWorth = new HashSet<int>();
        }

        public static CoinsStorage CreateCoinsStorage()
        {
            return new CoinsStorage();
        }

        public ICoinsStorageConfigurator SupplyWithCoins(int coinWorth, int quantity = 1)
        {
            if (!ValidateCoinWorth(coinWorth))
            {
                throw new Exception(_invalidCoinWorth);
            }

            if (quantity <= 0)
            {
                throw new Exception(_invalidCoinQuantity);
            }

            if (!_availableCoinsQuantity.ContainsKey(coinWorth))
            {
                _availableCoinsQuantity[coinWorth] = 0;
            }

            _availableCoinsQuantity[coinWorth] += quantity;
            return this;
        }

        public ICoinsStorageConfigurator ExtractCoins(int coinWorth, int quantity = 1)
        {
            if (!ValidateCoinWorth(coinWorth))
            {
                throw new Exception(_invalidCoinWorth);
            }

            if (quantity <= 0)
            {
                throw new Exception(_invalidCoinQuantity);
            }

            if (!_availableCoinsQuantity.ContainsKey(coinWorth))
            {
                _availableCoinsQuantity[coinWorth] = 0;
            }

            if (_availableCoinsQuantity[coinWorth] < quantity)
            {
                throw new Exception(_notEnoughCoins);
            }

            _availableCoinsQuantity[coinWorth] -= quantity;

            return this;
        }

        public ICoinsStorageConfigurator AddAcceptableCoinWorth(int cointWorth)
        {
            if (!_acceptableCoinsWorth.Contains(cointWorth))
            {
                _acceptableCoinsWorth.Add(cointWorth);
            }

            return this;
        }

        public ICoinsStorageConfigurator RemoveAcceptableCoinWorth(int cointWorth)
        {
            if (_acceptableCoinsWorth.Contains(cointWorth))
            {
                _acceptableCoinsWorth.Remove(cointWorth);
            }

            return this;
        }

        public List<int> ShowCurrentCoins()
        {
            return _availableCoinsQuantity.SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).ToList();
        }

        public int ShowTotalAmountInTheVault()
        {
            return ShowCurrentCoins().Sum();
        }

        public ICoinsStorage CloseCoinsVault()
        {
            return this;
        }

        public ICoinsStorageConfigurator OpenCoinsVault()
        {
            return this;
        }

        public List<int> GetAcceptableCoins()
        {
            return _acceptableCoinsWorth.ToList();
        }

        public bool ValidateCoinWorth(int cointWorth)
        {
            return _acceptableCoinsWorth.Contains(cointWorth);
        }

        public List<int> GetChange()
        {
            var changeCoins = _clientCoinsQuantity.SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).ToList();
            _clientCoinsQuantity.Clear();
            return changeCoins;
        }

        public int GetCurrentClientAmount()
        {
            return _clientCoinsQuantity.Sum(x => x.Key * x.Value);
        }

        public bool AddClientCoin(int coinWorth)
        {
            if (!ValidateCoinWorth(coinWorth))
            {
                return false;
            }

            if (!_clientCoinsQuantity.ContainsKey(coinWorth))
            {
                _clientCoinsQuantity[coinWorth] = 0;
            }

            _clientCoinsQuantity[coinWorth]++;
            return true;
        }

        public bool IsClientAmountEnough(int requestedAmount)
        {
            var clientTotalAmount = GetCurrentClientAmount();
            if (clientTotalAmount < requestedAmount)
            {
                return false;
            }

            return true;
        }

        public bool CheckForChange(int requestedAmount)
        {
            return ProcessRequest(requestedAmount, true);
        }

        public bool CompleteRequest(int requestedAmount)
        {
            return ProcessRequest(requestedAmount, false);
        }

        private bool ProcessRequest(int requestedAmount, bool testRunOnly)
        {
            if (!IsClientAmountEnough(requestedAmount))
            {
                return false;
            }

            var clientTotalAmount = GetCurrentClientAmount();
            var amountToPrepareAsChange = clientTotalAmount - requestedAmount;
            var totalCoinsQuantity = TemporaryMergeCoins();
            var availableCoinsQuantityLeft = new Dictionary<int, int>(totalCoinsQuantity);
            var clientCoinsQuantityLeft = new Dictionary<int, int>();

            foreach (var row in totalCoinsQuantity)
            {
                var coinsCount = row.Value;
                var coinsWorth = row.Key;
                while (coinsCount > 0 && amountToPrepareAsChange >= coinsWorth)
                {
                    amountToPrepareAsChange -= row.Key;
                    coinsCount--;
                    if (!clientCoinsQuantityLeft.ContainsKey(coinsWorth))
                    {
                        clientCoinsQuantityLeft[coinsWorth] = 0;
                    }

                    clientCoinsQuantityLeft[coinsWorth]++;
                }

                availableCoinsQuantityLeft[coinsWorth] = coinsCount;
            }

            if (amountToPrepareAsChange != 0)
            {
                return false;
            }

            if (!testRunOnly)
            {
                _availableCoinsQuantity = availableCoinsQuantityLeft;
                _clientCoinsQuantity = clientCoinsQuantityLeft;
            }

            return true;
        }

        private Dictionary<int, int> TemporaryMergeCoins()
        {
            var totalCoins = new Dictionary<int, int>(_availableCoinsQuantity);
            foreach (var row in _clientCoinsQuantity)
            {
                if (!totalCoins.ContainsKey(row.Key))
                {
                    totalCoins[row.Key] = 0;
                }

                totalCoins[row.Key] += row.Value;
            }

            return totalCoins;
        }
    }
}
