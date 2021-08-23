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
    public class IOProcessorTest
    {
        [TestMethod]
        public void When_ValidCoinInput_That_ParsedAmountIsExpected()
        {
            var ioProcessor = new IOProcessor(new ConsoleFacade());

            Assert.AreEqual(true, ioProcessor.ParseInputToPence(" £2", out int poundOutputInPence));
            Assert.AreEqual(poundOutputInPence, 200);

            Assert.AreEqual(true, ioProcessor.ParseInputToPence("50p", out int penceOutput));
            Assert.AreEqual(penceOutput, 50);
        }

        [TestMethod]
        public void When_InValidCoinInput_That_ResultIsNegative()
        {
            var ioProcessor = new IOProcessor(new ConsoleFacade());
            Assert.AreEqual(false, ioProcessor.ParseInputToPence(" £s2", out _));
            Assert.AreEqual(false, ioProcessor.ParseInputToPence("c50p", out _));
            Assert.AreEqual(false, ioProcessor.ParseInputToPence("50.p", out _));
        }

        [TestMethod]
        public void When_ValidItemInput_That_ParsedIdIsExpected()
        {
            var ioProcessor = new IOProcessor(new ConsoleFacade());

            Assert.AreEqual(true, ioProcessor.ParseInputToItemId("slot 13", out int itemId13));
            Assert.AreEqual(itemId13, 13);

            Assert.AreEqual(true, ioProcessor.ParseInputToItemId("slot14", out int itemId14));
            Assert.AreEqual(itemId14, 14);

            Assert.AreEqual(true, ioProcessor.ParseInputToItemId("slot       15", out int itemId15));
            Assert.AreEqual(itemId15, 15);

            Assert.AreEqual(true, ioProcessor.ParseInputToItemId("sLOt       16", out int itemId16));
            Assert.AreEqual(itemId16, 16);
        }

        [TestMethod]
        public void When_InValidItemInput_That_ResultIsNegative()
        {
            var ioProcessor = new IOProcessor(new ConsoleFacade());
            Assert.AreEqual(false, ioProcessor.ParseInputToItemId("Slot d1", out _));
            Assert.AreEqual(false, ioProcessor.ParseInputToItemId("s lot 2", out _));
            Assert.AreEqual(false, ioProcessor.ParseInputToItemId("slot. 3", out _));
        }

        [TestMethod]
        public void When_CallingAnyWrite_That_WriteLineIsCalled()
        {
            var moqConsoleFacade = new Moq.Mock<IConsoleFacade>();
            var ioProcessor = new IOProcessor(moqConsoleFacade.Object);

            ioProcessor.WriteAcceptableCoins(new List<int>() { 1 });
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(1));

            ioProcessor.WriteNotEnoughClientAmount(2);
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(2));

            ioProcessor.WriteNotEnoughChange();
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(3));

            ioProcessor.WriteNotEnoughQuantiy(1);
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(4));

            ioProcessor.WriteInvalidItem(1);
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(5));

            ioProcessor.WriteInvalidInput(string.Empty);
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(6));

            ioProcessor.WriteCannotAcceptClientCoin(200);
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(7));

            ioProcessor.WriteInvalidCointWorth(200);
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(8));

            ioProcessor.WriteClientTotalAmount(200);
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(9));

            ioProcessor.WriteChange(new List<int>() { 1 });
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(10));

            ioProcessor.WriteItemOutput(new VendingItem(string.Empty, 0));
            moqConsoleFacade.Verify(mock => mock.WriteLine(It.IsAny<string>()), Times.Exactly(11));
        }
    }
}
