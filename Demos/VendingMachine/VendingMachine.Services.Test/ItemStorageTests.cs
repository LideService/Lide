using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VendingMachine.Services.Model;
using VendingMachine.Services.Services;
using VendingMachine.Services.Test.Helpers;

namespace VendingMachine.Services.Test
{
    [TestClass]
    public class ItemStorageTests
    {
        [TestMethod]
        public void When_AddingNewItemIntoStorage_That_NoExceptionWillBeThrown()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(10);

            ExcAssert.NotThrown(() => itemStorage.ConfigureItem(vendingItem, 10));
        }

        [TestMethod]
        public void When_AddingItemOverTheStorageLimit_That_ExceptionWillBeThrown()
        {
            var vendingItem = new VendingItem("1", 10);
            var vendingItem2 = new VendingItem("2", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1);
            itemStorage.ConfigureItem(vendingItem, 10);

            ExceptionAssert.Throws<Exception>(() => itemStorage.ConfigureItem(vendingItem2, 10));
        }

        [TestMethod]
        public void When_AddingItemWithNegativeQuantity_That_ExceptionWillBeThrown()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1);

            ExceptionAssert.Throws<Exception>(() => itemStorage.ConfigureItem(vendingItem, -10));
        }

        [TestMethod]
        public void When_ChaningItemQuantityToPositive_That_NoExceptionWillBeThrown()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1);
            itemStorage.ConfigureItem(vendingItem, 10);

            ExcAssert.NotThrown(() => itemStorage.ChangeItemQuantity(vendingItem, -8));
        }

        [TestMethod]
        public void When_ChaningItemQuantityToPositive_That_QuantityWillChangeAccordingly()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1);
            itemStorage.ConfigureItem(vendingItem, 10);

            var beforeChangeItemsQuantity = itemStorage.CloseItemStorage().GetStorageContentWithQuantity();

            itemStorage.ChangeItemQuantity(vendingItem, -8);

            var afterChangeItemsQuantity = itemStorage.CloseItemStorage().GetStorageContentWithQuantity();

            Assert.AreEqual(beforeChangeItemsQuantity[vendingItem], 10);
            Assert.AreEqual(afterChangeItemsQuantity[vendingItem], 2);
        }

        [TestMethod]
        public void When_ChaningItemQuantityToNegative_That_ExceptionWillBeThrown()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1);
            itemStorage.ConfigureItem(vendingItem, 10);

            ExceptionAssert.Throws<Exception>(() => itemStorage.ChangeItemQuantity(vendingItem, -12));
        }

        [TestMethod]
        public void When_RemovingItem_That_ItWontExistInTheStorageAnymore()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1);
            itemStorage.ConfigureItem(vendingItem, 10);

            var beforeItemRemoveQuantity = itemStorage.CloseItemStorage().GetStorageContentWithQuantity();
            itemStorage.RemoveItem(vendingItem);
            var afterItemRemoveQuantity = itemStorage.CloseItemStorage().GetStorageContentWithQuantity();

            Assert.AreEqual(true, beforeItemRemoveQuantity.ContainsKey(vendingItem));
            Assert.AreEqual(false, afterItemRemoveQuantity.ContainsKey(vendingItem));
        }

        [TestMethod]
        public void When_RemovingNonExistantItem_That_NoExceptionWillBeThrown()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1);
            itemStorage.ConfigureItem(vendingItem, 10);

            itemStorage.RemoveItem(vendingItem);

            ExcAssert.NotThrown(() => itemStorage.RemoveItem(vendingItem));
        }

        [TestMethod]
        public void When_ItemExists_That_CheckForItemIsPositive()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1)
                .ConfigureItem(vendingItem, 10)
                .CloseItemStorage();

            Assert.AreEqual(true, itemStorage.CheckForItem(vendingItem.Id, out VendingItem dummy));
        }

        [TestMethod]
        public void When_ItemDoesntExists_That_CheckForItemIsNegative()
        {
            var invalidItemId = -1;
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1)
                .ConfigureItem(vendingItem, 10)
                .CloseItemStorage();

            Assert.AreEqual(false, itemStorage.CheckForItem(invalidItemId, out VendingItem dummy));
        }

        [TestMethod]
        public void When_ItemWithEnoughQuantity_That_CheckForQuantityIsPositive()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1)
                .ConfigureItem(vendingItem, 10)
                .CloseItemStorage();

            Assert.AreEqual(true, itemStorage.CheckForQuantity(vendingItem, 8));
        }

        [TestMethod]
        public void When_ItemWithNotEnoughQuantity_That_CheckForQuantityIsNegatiev()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1)
                .ConfigureItem(vendingItem, 10)
                .CloseItemStorage();

            Assert.AreEqual(false, itemStorage.CheckForQuantity(vendingItem, 12));
        }

        [TestMethod]
        public void When_ItemWithEnoughQuantity_That_GetItemFromStorageIsPositive()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1)
                .ConfigureItem(vendingItem, 10)
                .CloseItemStorage();

            Assert.AreEqual(true, itemStorage.GetItemFromStorage(vendingItem));
        }

        [TestMethod]
        public void When_GettingLastItemFromStorage_That_CannotTakeAnymore()
        {
            var vendingItem = new VendingItem("1", 10);
            var itemStorage = ItemsStorage.CreateItemStorage(1)
                .ConfigureItem(vendingItem, 1)
                .CloseItemStorage();

            Assert.AreEqual(true, itemStorage.GetItemFromStorage(vendingItem));
            Assert.AreEqual(false, itemStorage.GetItemFromStorage(vendingItem));
        }
    }
}
