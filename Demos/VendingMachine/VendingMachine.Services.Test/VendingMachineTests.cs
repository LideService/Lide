using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VendingMachine.Services.Contracts;
using VendingMachine.Services.Facades;
using VendingMachine.Services.Model;
using VendingMachine.Services.Services;

namespace VendingMachine.Services.Test
{
    [TestClass]
    public class VendingMachineTests
    {
        [TestMethod]
        public void When_InsertingExactCoins_That_ItemCanBeBoughtWithNoChange()
        {
            var moqProcessorObj = new Mock<IoProcessor>(new ConsoleFacade()) { CallBase = true };
            var moqProcessor = moqProcessorObj.As<IIoProcessor>();

            var item1 = new VendingItem("Item 1", 185);
            var item2 = new VendingItem("Item 2", 135);

            var coinsStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(200)
                .AddAcceptableCoinWorth(100)
                .AddAcceptableCoinWorth(50)
                .AddAcceptableCoinWorth(20)
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(5)
                .SupplyWithCoins(200, 7)
                .SupplyWithCoins(100, 7)
                .SupplyWithCoins(50, 7)
                .SupplyWithCoins(20, 7)
                .SupplyWithCoins(10, 7)
                .SupplyWithCoins(5, 7)
                .CloseCoinsVault();

            var itemStorage = ItemsStorage.CreateItemStorage(5)
                .ConfigureItem(item1, 7)
                .ConfigureItem(item2, 7)
                .CloseItemStorage();

            var vendingMachine = Services.VendingMachine
                .CreateVendingMachine(moqProcessor.Object, coinsStorage, itemStorage)
                .StartVendingMachine();

            vendingMachine.ProcessInput("£1");
            vendingMachine.ProcessInput("50p");
            vendingMachine.ProcessInput("20p");
            vendingMachine.ProcessInput("10p");
            vendingMachine.ProcessInput("5p");
            vendingMachine.ProcessInput($"slot {item1.Id}");

            moqProcessor.Verify(mock => mock.WriteChange(It.IsAny<List<int>>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteClientTotalAmount(It.IsAny<int>()), Times.Exactly(5));
            moqProcessor.Verify(mock => mock.WriteItemOutput(It.IsAny<VendingItem>()), Times.Once);

            moqProcessor.Verify(mock => mock.WriteStorageContent(It.IsAny<Dictionary<VendingItem, int>>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteInvalidCoinsWorth(It.IsAny<int>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteCannotAcceptClientCoin(It.IsAny<int>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteInvalidInput(It.IsAny<string>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteInvalidItem(It.IsAny<int>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteNotEnoughChange(), Times.Never);
            moqProcessor.Verify(mock => mock.WriteNotEnoughClientAmount(It.IsAny<int>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteAcceptableCoins(It.IsAny<List<int>>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteNotEnoughQuantity(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void When_InsertingCoins_That_VerificationsWorks()
        {
            var moqProcessorObj = new Mock<IoProcessor>(new ConsoleFacade()) { CallBase = true };
            var moqProcessor = moqProcessorObj.As<IIoProcessor>();

            var item1 = new VendingItem("Item 1", 185);
            var item2 = new VendingItem("Item 2", 135);

            var coinsStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(200)
                .AddAcceptableCoinWorth(100)
                .AddAcceptableCoinWorth(50)
                .AddAcceptableCoinWorth(20)
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(5)
                .SupplyWithCoins(200, 7)
                .SupplyWithCoins(100, 7)
                .SupplyWithCoins(50, 7)
                .SupplyWithCoins(20, 7)
                .SupplyWithCoins(10, 7)
                .SupplyWithCoins(5, 7)
                .CloseCoinsVault();

            var itemStorage = ItemsStorage.CreateItemStorage(5)
                .ConfigureItem(item1, 7)
                .ConfigureItem(item2, 7)
                .CloseItemStorage();

            var vendingMachine = Services.VendingMachine
                .CreateVendingMachine(moqProcessor.Object, coinsStorage, itemStorage)
                .StartVendingMachine();

            vendingMachine.ProcessInput("£d2");
            vendingMachine.ProcessInput("£s2");
            vendingMachine.ProcessInput("£12");
            vendingMachine.ProcessInput("£42");
            vendingMachine.ProcessInput("slot 7");

            moqProcessor.Verify(mock => mock.WriteChange(It.IsAny<List<int>>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteClientTotalAmount(It.IsAny<int>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteItemOutput(It.IsAny<VendingItem>()), Times.Never);

            moqProcessor.Verify(mock => mock.WriteStorageContent(It.IsAny<Dictionary<VendingItem, int>>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteInvalidCoinsWorth(It.IsAny<int>()), Times.Exactly(2));
            moqProcessor.Verify(mock => mock.WriteCannotAcceptClientCoin(It.IsAny<int>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteInvalidInput(It.IsAny<string>()), Times.Exactly(2));
            moqProcessor.Verify(mock => mock.WriteInvalidItem(It.IsAny<int>()), Times.Once);
            moqProcessor.Verify(mock => mock.WriteNotEnoughChange(), Times.Never);
            moqProcessor.Verify(mock => mock.WriteNotEnoughClientAmount(It.IsAny<int>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteAcceptableCoins(It.IsAny<List<int>>()), Times.Never);
            moqProcessor.Verify(mock => mock.WriteNotEnoughQuantity(It.IsAny<int>()), Times.Never);
        }
    }
}
