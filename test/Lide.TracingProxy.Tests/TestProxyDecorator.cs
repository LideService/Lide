using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lide.TracingProxy.Tests
{
    [TestClass]
    public class TestProxyDecorator
    {
        [TestMethod]
        public void That_OriginalObjectIsReturned_When_NoDecoratorsAreProvided()
        {
            var originalObject = new Tester();
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject, false);
            var decorated = proxyDecorator.GetDecoratedObject();
            Assert.IsTrue(ReferenceEquals(originalObject, decorated));
        }
        
        [TestMethod]
        public void That_DecoratedObjectIsReturned_When_ReadonlyDecoratorIsProvided()
        {
            var originalObject = new Tester();
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject, false);
            proxyDecorator.SetDecorator(new DecoratorReadonly());
            var decorated = proxyDecorator.GetDecoratedObject();
            Assert.IsFalse(ReferenceEquals(originalObject, decorated));
        }
        
        [TestMethod]
        public void That_DecoratedObjectIsReturned_When_VolatileDecoratorIsProvided()
        {
            var originalObject = new Tester();
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject, false);
            proxyDecorator.SetDecorator(new DecoratorVolatile());
            var decorated = proxyDecorator.GetDecoratedObject();
            Assert.IsFalse(ReferenceEquals(originalObject, decorated));
        }
        
        
        [TestMethod]
        public void That_GeneratedTypeIsUsed_When_TheSameInterfaceIsProxied()
        {
            var originalObject = new Tester();
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject, false);
            proxyDecorator.SetDecorator(new DecoratorVolatile());
            var decorated = proxyDecorator.GetDecoratedObject();
            
            var proxyDecorator2 = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator2.SetOriginalObject(originalObject, false);
            proxyDecorator2.SetDecorator(new DecoratorVolatile());
            var decorated2 = proxyDecorator2.GetDecoratedObject();
            Assert.AreEqual(decorated.GetType(), decorated2.GetType());
            Assert.IsFalse(ReferenceEquals(decorated, decorated2));
        }

        [TestMethod]
        public void That_DecoratorsAreInvokedInProperOrder_When_ProxyIsUsed()
        {
            var expected = new []{ 0, 1, 2, 3, 4, 5, 6, 7, };
            var actual = new int[8];
            var readonlyDecorator1 = new Mock<IObjectDecoratorReadonly>();
            var readonlyDecorator2 = new Mock<IObjectDecoratorReadonly>();
            var volatileDecorator1 = new Mock<IObjectDecoratorVolatile>();
            var volatileDecorator2 = new Mock<IObjectDecoratorVolatile>();
            readonlyDecorator1.Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadata>())).Callback(() => actual[0] = 0);
            readonlyDecorator2.Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadata>())).Callback(() => actual[1] = 1);
            volatileDecorator1.Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>())).Callback(() => actual[2] = 2);
            volatileDecorator2.Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>())).Callback(() => actual[3] = 3);
            readonlyDecorator1.Setup(x => x.ExecuteAfterResult(It.IsAny<MethodMetadata>())).Callback(() => actual[4] = 4);
            readonlyDecorator2.Setup(x => x.ExecuteAfterResult(It.IsAny<MethodMetadata>())).Callback(() => actual[5] = 5);
            volatileDecorator1.Setup(x => x.ExecuteAfterResult(It.IsAny<MethodMetadataVolatile>())).Callback(() => actual[6] = 6);
            volatileDecorator2.Setup(x => x.ExecuteAfterResult(It.IsAny<MethodMetadataVolatile>())).Callback(() => actual[7] = 7);

            var originalObject = new Tester();
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject, false);
            proxyDecorator.SetDecorator(readonlyDecorator1.Object);
            proxyDecorator.SetDecorator(volatileDecorator1.Object);
            proxyDecorator.SetDecorator(volatileDecorator2.Object);
            proxyDecorator.SetDecorator(readonlyDecorator2.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            decorated.Method();
            
            readonlyDecorator1.Verify(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadata>()), Times.Once);
            readonlyDecorator1.Verify(x => x.ExecuteAfterResult(It.IsAny<MethodMetadata>()), Times.Once);
            readonlyDecorator2.Verify(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadata>()), Times.Once);
            readonlyDecorator2.Verify(x => x.ExecuteAfterResult(It.IsAny<MethodMetadata>()), Times.Once);
            volatileDecorator1.Verify(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>()), Times.Once);
            volatileDecorator1.Verify(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>()), Times.Once);
            volatileDecorator2.Verify(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>()), Times.Once);
            volatileDecorator2.Verify(x => x.ExecuteAfterResult(It.IsAny<MethodMetadataVolatile>()), Times.Once);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        private interface ITester
        {
            void Method();
        }

        private class Tester : ITester
        {
            public void Method()
            {
            }
        }
        
        private class DecoratorReadonly : IObjectDecoratorReadonly
        {
            public string Id { get; }
        }
        
        private class DecoratorVolatile : IObjectDecoratorVolatile
        {
            public string Id { get; }
        }
    }

}