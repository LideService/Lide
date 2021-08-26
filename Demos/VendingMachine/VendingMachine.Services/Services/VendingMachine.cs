using VendingMachine.Services.Contracts;

namespace VendingMachine.Services.Services
{
    public class VendingMachine : IVendingMachineConfigurator, IVendingMachine
    {
        private readonly IIoProcessor _ioProcessor;
        private readonly IItemsStorage _itemsStorage;
        private readonly ICoinsStorage _coinsStorage;

        private VendingMachine(
            IIoProcessor ioProcessor,
            ICoinsStorage coinsStorage,
            IItemsStorage itemsStorage)
        {
            _ioProcessor = ioProcessor;
            _coinsStorage = coinsStorage;
            _itemsStorage = itemsStorage;
        }

        public static IVendingMachineConfigurator CreateVendingMachine(
            IIoProcessor ioProcessor,
            ICoinsStorage coinsStorage,
            IItemsStorage itemsStorage)
        {
            return new VendingMachine(ioProcessor, coinsStorage, itemsStorage);
        }

        public ICoinsStorageConfigurator OpenCoinsStorage()
        {
            return _coinsStorage.OpenCoinsVault();
        }

        public IItemsStorageConfigurator OpenItemsStorage()
        {
            return _itemsStorage.OpenItemStorage();
        }

        public IVendingMachine StartVendingMachine()
        {
            return this;
        }

        public IVendingMachineConfigurator StopVendingMachine()
        {
            return this;
        }

        public void ProcessInput(string input)
        {
            if (_ioProcessor.ParseInputToPence(input, out int coinWorthInPence))
            {
                InsertCoin(coinWorthInPence);
            }
            else
            if (_ioProcessor.ParseInputToItemId(input, out int itemId))
            {
                ProcessItemRequest(itemId);
            }
            else
            {
                _ioProcessor.WriteInvalidInput(input);
            }
        }

        public void PrintStartup()
        {
            _ioProcessor.WriteStorageContent(_itemsStorage.GetStorageContentWithQuantity());
            _ioProcessor.WriteAcceptableCoins(_coinsStorage.GetAcceptableCoins());
        }

        private void InsertCoin(int coinWorth)
        {
            if (!_coinsStorage.ValidateCoinWorth(coinWorth))
            {
                _ioProcessor.WriteInvalidCoinsWorth(coinWorth);
                return;
            }

            if (!_coinsStorage.AddClientCoin(coinWorth))
            {
                _ioProcessor.WriteCannotAcceptClientCoin(coinWorth);
                return;
            }

            var clientTotalAmount = _coinsStorage.GetCurrentClientAmount();
            _ioProcessor.WriteClientTotalAmount(clientTotalAmount);
        }

        private void ProcessItemRequest(int itemId)
        {
            if (!_itemsStorage.CheckForItem(itemId, out var vendingItem))
            {
                _ioProcessor.WriteInvalidItem(itemId);
                return;
            }

            if (!_itemsStorage.CheckForQuantity(vendingItem))
            {
                _ioProcessor.WriteNotEnoughQuantity(itemId);
                return;
            }

            if (!_coinsStorage.IsClientAmountEnough(vendingItem.CostInPence))
            {
                var clientTotalAmount = _coinsStorage.GetCurrentClientAmount();
                _ioProcessor.WriteNotEnoughClientAmount(vendingItem.CostInPence);
                _ioProcessor.WriteClientTotalAmount(clientTotalAmount);
                return;
            }

            if (!_coinsStorage.CheckForChange(vendingItem.CostInPence))
            {
                _ioProcessor.WriteNotEnoughChange();
                return;
            }

            _coinsStorage.CompleteRequest(vendingItem.CostInPence);

            var change = _coinsStorage.GetChange();
            if (change.Count > 0)
            {
                _ioProcessor.WriteChange(change);
            }

            _ioProcessor.WriteItemOutput(vendingItem);
        }
    }
}
