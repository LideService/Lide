using Lide.TracingProxy.Contract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lide.TracingProxy.Tests
{
    [TestClass]
    public class TestProxyDecoratorExecutions
    {
        [TestMethod]
        public void That_MetadataIsPopulatedProperly_WhenProxyIsUsed()
        {
            var readonlyDecorator = new Mock<IObjectDecoratorReadonly>();
            var originalObject = new Mock<ITester>();
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(readonlyDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();

            var value1 = "Empty";
            var value2 = 3;
            decorated.Method(value1, value2);
        }

        [TestMethod]
        public void That_NothingWillBreak_When_DecoratorThrows()
        {
        }
        
        [TestMethod]
        public void That_ExceptionIsNotWrapped_When_BaseObjectThrows()
        {
        }
        
        [TestMethod]
        public void That_InputValuesCanBeChanged_When_SetInExecuteBeforeInvoke()
        {
        }
        
        [TestMethod]
        public void That_OutputCanBeChanged_When_SetInExecuteBeforeInvoke_WithoutInvokingBaseMethod()
        {
        }
        
        [TestMethod]
        public void That_ExceptionCanBeRaised_When_SetInExecuteBeforeInvoke_WithoutInvokingBaseMethod()
        {
        }
        
        [TestMethod]
        public void That_OutputValueCanBeChanged_SetInExecuteAfterResult()
        {
        }
        
        [TestMethod]
        public void That_ExceptionCanBeRaised_When_SetInExecuteAfterResult()
        {
        }
        
        [TestMethod]
        public void That_InputChange_When_SetInExecuteAfterResult_WillEditOriginalInput()
        {
        }

        private interface ITester
        {
            void Method(string value, int data);
        }
    }
}