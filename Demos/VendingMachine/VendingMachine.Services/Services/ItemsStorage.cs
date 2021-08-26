using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Services.Contracts;
using VendingMachine.Services.Model;

namespace VendingMachine.Services.Services
{
    public class ItemsStorage : IItemsStorageConfigurator, IItemsStorage
    {
        private const string CantAddNewItem = "Can't add new item. Storage is full!";
        private const string CantConfigureItemWithNegativeQuantity = "Can't configure item to have negative quantity!";
        private const int InvalidItemId = -1;
        private readonly int _storageSize;
        private readonly Dictionary<int, VendingItem> _vendingItems;
        private readonly Dictionary<int, int> _vendingItemsQuantity;

        private ItemsStorage(int storageSize)
        {
            _storageSize = storageSize;
            _vendingItems = new Dictionary<int, VendingItem>();
            _vendingItemsQuantity = new Dictionary<int, int>();
        }

        public static IItemsStorageConfigurator CreateItemStorage(int storageSize)
        {
            return new ItemsStorage(storageSize);
        }

        public IItemsStorageConfigurator ConfigureItem(VendingItem vendingItem, int? quantity = null)
        {
            if (!_vendingItems.ContainsKey(vendingItem.Id) && _vendingItems.Count == _storageSize)
            {
                throw new Exception(CantAddNewItem);
            }

            if (!_vendingItems.ContainsKey(vendingItem.Id))
            {
                _vendingItems[vendingItem.Id] = vendingItem;
                _vendingItemsQuantity[vendingItem.Id] = 0;
            }

            if (quantity.HasValue)
            {
                if (quantity < 0 && !CheckForQuantity(vendingItem, quantity.Value * (-1)))
                {
                    throw new Exception(CantConfigureItemWithNegativeQuantity);
                }

                ChangeItemQuantity(vendingItem, quantity.Value);
            }

            return this;
        }

        public IItemsStorageConfigurator RemoveItem(VendingItem vendingItem)
        {
            if (_vendingItems.ContainsKey(vendingItem.Id))
            {
                _vendingItems.Remove(vendingItem.Id);
                _vendingItemsQuantity.Remove(vendingItem.Id);
            }

            return this;
        }

        public IItemsStorageConfigurator ChangeItemQuantity(VendingItem vendingItem, int quantity)
        {
            if (quantity < 0 && !CheckForQuantity(vendingItem, -quantity))
            {
                throw new Exception(CantConfigureItemWithNegativeQuantity);
            }

            ChangeQuantity(vendingItem.Id, quantity);
            return this;
        }

        public IItemsStorage CloseItemStorage()
        {
            return this;
        }

        public IItemsStorageConfigurator OpenItemStorage()
        {
            return this;
        }

        public Dictionary<VendingItem, int> GetStorageContentWithQuantity()
        {
            return _vendingItemsQuantity.ToDictionary(x => _vendingItems[x.Key], x => x.Value);
        }

        public bool CheckForItem(int itemId, out VendingItem vendingItem)
        {
            if (!_vendingItems.ContainsKey(itemId))
            {
                vendingItem = null;
                return false;
            }

            vendingItem = _vendingItems[itemId];
            return true;
        }

        public bool CheckForQuantity(VendingItem vendingItem, int quantityNeeded = 1)
        {
            if (!CheckForItem(vendingItem?.Id ?? InvalidItemId, out var dummy))
            {
                return false;
            }

            return _vendingItemsQuantity[vendingItem!.Id] - quantityNeeded >= 0;
        }

        public bool GetItemFromStorage(VendingItem vendingItem)
        {
            if (!CheckForQuantity(vendingItem))
            {
                return false;
            }

            ChangeQuantity(vendingItem.Id, -1);
            return true;
        }

        private void ChangeQuantity(int itemId, int quantity)
        {
            _vendingItemsQuantity[itemId] += quantity;
        }
    }
}
