using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VendingMachine.Services.Services;
using VendingMachine.Services.Test.Helpers;

namespace VendingMachine.Services.Test
{
    [TestClass]
    public class CoinsStorageTests
    {
        [TestMethod]
        public void When_SupplyWithValidCoins_That_NoExceptionWillBeThrown()
        {
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(5)
                .CloseCoinsVault();

            ExcAssert.NotThrown(() =>
                coinStorage.OpenCoinsVault()
                    .SupplyWithCoins(10, 2)
                    .SupplyWithCoins(5, 3));
        }

        [TestMethod]
        public void When_SupplyWithInValidCoins_That_ExceptionWillBeThrown()
        {
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(5)
                .CloseCoinsVault();

            Assert.ThrowsException<Exception>(() =>
                coinStorage.OpenCoinsVault()
                    .SupplyWithCoins(20, 2));
        }

        [TestMethod]
        public void When_ExtractValidCoins_That_NoExceptionWillBeThrown()
        {
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(5)
                .SupplyWithCoins(10, 19)
                .SupplyWithCoins(5, 13)
                .CloseCoinsVault();

            ExcAssert.NotThrown(() =>
                coinStorage.OpenCoinsVault()
                    .ExtractCoins(10)
                    .ExtractCoins(5, 13));
        }

        [TestMethod]
        public void When_ExtractInValidCoins_That_ExceptionWillBeThrown()
        {
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(5)
                .SupplyWithCoins(10, 19)
                .SupplyWithCoins(5, 13)
                .CloseCoinsVault();

            Assert.ThrowsException<Exception>(() =>
                coinStorage.OpenCoinsVault()
                    .ExtractCoins(20));
        }

        [TestMethod]
        public void When_ExtractCoinsMoreThanAvailableQuantity_That_ExceptionWillBeThrown()
        {
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .SupplyWithCoins(10, 19)
                .CloseCoinsVault();

            Assert.ThrowsException<Exception>(() =>
                coinStorage.OpenCoinsVault()
                    .ExtractCoins(10, 24));
        }

        [TestMethod]
        public void When_SupplyingCoinsWithNegativeQuantity_That_ExceptionWillBeThrown()
        {
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .SupplyWithCoins(10, 19)
                .CloseCoinsVault();

            Assert.ThrowsException<Exception>(() =>
                coinStorage.OpenCoinsVault()
                    .ExtractCoins(10, -5));
        }

        [TestMethod]
        public void When_ExtractCoinsWithNegativeQuantity_That_ExceptionWillBeThrown()
        {
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .SupplyWithCoins(10, 19)
                .CloseCoinsVault();

            Assert.ThrowsException<Exception>(() =>
                coinStorage.OpenCoinsVault()
                    .ExtractCoins(10, -5));
        }

        [TestMethod]
        public void When_SupplyWithCoinsValidCoins_That_CurrentCoinsWillIncreaseAccordingly()
        {
            var expectedChangeInAmount = 120;
            var expectedChangeInCount = 12;

            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(20)
                .SupplyWithCoins(10, 7)
                .SupplyWithCoins(20, 13)
                .CloseCoinsVault();

            var beforeAddingMoreTotalAmount = coinStorage.OpenCoinsVault().ShowTotalAmountInTheVault();
            var beforeAddingMoreTotalCoinsCount = coinStorage.OpenCoinsVault().ShowCurrentCoins().Count;

            coinStorage.OpenCoinsVault()
                .SupplyWithCoins(10, 12)
                .CloseCoinsVault();

            var afterAddingMoreTotalAmount = coinStorage.OpenCoinsVault().ShowTotalAmountInTheVault();
            var afterAddingMoreTotalCoinsCount = coinStorage.OpenCoinsVault().ShowCurrentCoins().Count;

            Assert.AreEqual(beforeAddingMoreTotalAmount + expectedChangeInAmount, afterAddingMoreTotalAmount);
            Assert.AreEqual(beforeAddingMoreTotalCoinsCount + expectedChangeInCount, afterAddingMoreTotalCoinsCount);
        }

        [TestMethod]
        public void When_ExtractValidCoins_That_CurrentCoinsWillDecreaseAccordingly()
        {
            var expectedChangeInAmount = -120;
            var expectedChangeInCount = -12;

            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(20)
                .SupplyWithCoins(10, 31)
                .SupplyWithCoins(20, 13)
                .CloseCoinsVault();

            var beforeAddingMoreTotalAmount = coinStorage.OpenCoinsVault().ShowTotalAmountInTheVault();
            var beforeAddingMoreTotalCoinsCount = coinStorage.OpenCoinsVault().ShowCurrentCoins().Count;

            coinStorage.OpenCoinsVault()
                .ExtractCoins(10, 12)
                .CloseCoinsVault();

            var afterAddingMoreTotalAmount = coinStorage.OpenCoinsVault().ShowTotalAmountInTheVault();
            var afterAddingMoreTotalCoinsCount = coinStorage.OpenCoinsVault().ShowCurrentCoins().Count;

            Assert.AreEqual(beforeAddingMoreTotalAmount + expectedChangeInAmount, afterAddingMoreTotalAmount);
            Assert.AreEqual(beforeAddingMoreTotalCoinsCount + expectedChangeInCount, afterAddingMoreTotalCoinsCount);
        }

        [TestMethod]
        public void When_ClientInsertCoins_That_ClientAmountIsIncreasedAccordingly()
        {
            var expectedChangeInClientAmount = 90;
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(20)
                .SupplyWithCoins(10, 31)
                .SupplyWithCoins(20, 13)
                .CloseCoinsVault();

            var beforeClientAddingTotalAmount = coinStorage.OpenCoinsVault().ShowTotalAmountInTheVault();
            var beforeClientAddingTotalCoinsCount = coinStorage.OpenCoinsVault().ShowCurrentCoins().Count;
            var beforeClientAddingTotalClientAmount = coinStorage.GetCurrentClientAmount();

            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);

            var afterClientAddingTotalAmount = coinStorage.OpenCoinsVault().ShowTotalAmountInTheVault();
            var afterClientAddingTotalCoinsCount = coinStorage.OpenCoinsVault().ShowCurrentCoins().Count;
            var afterClientAddingTotalClientAmount = coinStorage.GetCurrentClientAmount();

            Assert.AreEqual(beforeClientAddingTotalAmount, afterClientAddingTotalAmount);
            Assert.AreEqual(beforeClientAddingTotalCoinsCount, afterClientAddingTotalCoinsCount);
            Assert.AreEqual(beforeClientAddingTotalClientAmount + expectedChangeInClientAmount, afterClientAddingTotalClientAmount);
        }

        [TestMethod]
        public void When_ClientInsertExactCoins_That_TheExactAmountCanBeProcessed()
        {
            var clientAmountAsExactRequestAmount = 90;
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(20)
                .SupplyWithCoins(10, 31)
                .SupplyWithCoins(20, 13)
                .CloseCoinsVault();

            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);

            Assert.AreEqual(true, coinStorage.CheckForChange(clientAmountAsExactRequestAmount));
            Assert.AreEqual(true, coinStorage.CompleteRequest(clientAmountAsExactRequestAmount));
        }

        [TestMethod]
        public void When_AmountIsProcessed_That_CoinsInTheVaultIncreaseAccordingly()
        {
            var clientAmountAsExactRequestAmount = 90;
            var clientCoinsCount = 7;
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(20)
                .SupplyWithCoins(10, 31)
                .SupplyWithCoins(20, 13)
                .CloseCoinsVault();

            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);

            var beforeCompleteRequestTotalAmount = coinStorage.OpenCoinsVault().ShowTotalAmountInTheVault();
            var beforeCompleteRequestTotalCoinsCount = coinStorage.OpenCoinsVault().ShowCurrentCoins().Count;

            coinStorage.CheckForChange(clientAmountAsExactRequestAmount);
            coinStorage.CompleteRequest(clientAmountAsExactRequestAmount);

            var afterCompleteRequestTotalAmount = coinStorage.OpenCoinsVault().ShowTotalAmountInTheVault();
            var afterCompleteRequestTotalCoinsCount = coinStorage.OpenCoinsVault().ShowCurrentCoins().Count;
            var afterCompleteRequestTotalClientAmount = coinStorage.GetCurrentClientAmount();

            Assert.AreEqual(beforeCompleteRequestTotalAmount + clientAmountAsExactRequestAmount, afterCompleteRequestTotalAmount);
            Assert.AreEqual(beforeCompleteRequestTotalCoinsCount + clientCoinsCount, afterCompleteRequestTotalCoinsCount);
            Assert.AreEqual(afterCompleteRequestTotalClientAmount, 0);
        }

        [TestMethod]
        public void When_ClientInsertCoins_With_NotEnoughCoinsForChange_That_AmountCannotBeProcessed()
        {
            var clientAmountAsExactRequestAmount = 85;
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(20)
                .SupplyWithCoins(10, 31)
                .SupplyWithCoins(20, 13)
                .CloseCoinsVault();

            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);

            Assert.AreEqual(false, coinStorage.CheckForChange(clientAmountAsExactRequestAmount));
            Assert.AreEqual(false, coinStorage.CompleteRequest(clientAmountAsExactRequestAmount));
        }

        [TestMethod]
        public void When_ClientInsertCoins_After_ProcessingSomeAmount_That_TheChangeWillBeCorrect()
        {
            var clientAmountAsExactRequestAmount = 85;
            var expectedChangeAmount = 5;
            var coinStorage = CoinsStorage.CreateCoinsStorage()
                .AddAcceptableCoinWorth(5)
                .AddAcceptableCoinWorth(10)
                .AddAcceptableCoinWorth(20)
                .SupplyWithCoins(5, 31)
                .SupplyWithCoins(10, 31)
                .SupplyWithCoins(20, 13)
                .CloseCoinsVault();

            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(10);
            coinStorage.AddClientCoin(20);
            coinStorage.AddClientCoin(10);

            coinStorage.CheckForChange(clientAmountAsExactRequestAmount);
            coinStorage.CompleteRequest(clientAmountAsExactRequestAmount);
            var changeAmount = coinStorage.GetChange().Sum();

            Assert.AreEqual(changeAmount, expectedChangeAmount);
        }
    }
}
