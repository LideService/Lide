using System;
using System.Linq;
using Lide.TracingProxy.Contract;
using Lide.TracingProxy.Model;
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
            var value1 = "Empty";
            var value2 = 3;
            var value3 = 13;
            var callId = 1;
            var hashCode = typeof(ITester).GetMethods().First(x => x.Name == "Method").GetHashCode();
            var readonlyDecorator = new Mock<IObjectDecoratorReadonly>();
            var originalObject = new Mock<ITester>();
            MethodMetadata capturedMetadataBefore = null;
            MethodMetadata capturedMetadataAfter = null;

            originalObject
                .Setup(x => x.Method(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(value3);
            
            readonlyDecorator
                .Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadata>()))
                .Callback<MethodMetadata>((metadata) => capturedMetadataBefore = metadata);
            readonlyDecorator
                .Setup(x => x.ExecuteAfterResult(It.IsAny<MethodMetadata>()))
                .Callback<MethodMetadata>((metadata) => capturedMetadataAfter = metadata);

            void Verify(MethodMetadata metadata, bool after = false)
            {
                Assert.IsNotNull(metadata);
                Assert.AreEqual(callId, metadata.CallId);
                Assert.AreEqual(hashCode, metadata.MethodInfo.GetHashCode());
                Assert.AreEqual(originalObject.Object, metadata.PlainObject);
                Assert.IsNotNull(metadata.ParametersMetadata);
                CollectionAssert.AreEqual(new object[] {value1, value2}, metadata.ParametersMetadata.GetOriginalParameters());
                CollectionAssert.AreEqual(new object[] {value1, value2}, metadata.ParametersMetadata.GetEditedParameters());
                Assert.IsNotNull(metadata.ReturnMetadata);
                Assert.IsNull(metadata.ReturnMetadata.GetOriginalException());
                Assert.IsNull(metadata.ReturnMetadata.GetEditedException());
                if (after)
                {
                    Assert.AreEqual(value3, metadata.ReturnMetadata.GetOriginalResult());
                    Assert.AreEqual(value3, metadata.ReturnMetadata.GetEditedResult());
                }
                else
                {
                    Assert.IsNull(metadata.ReturnMetadata.GetOriginalResult());
                    Assert.IsNull(metadata.ReturnMetadata.GetEditedResult());
                }
            }
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(readonlyDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            
            Assert.AreEqual(value3, decorated.Method(value1, value2));
            Verify(capturedMetadataBefore);
            Verify(capturedMetadataAfter, true);
            callId++;
            Assert.AreEqual(value3, decorated.Method(value1, value2));
            Verify(capturedMetadataBefore);
            Verify(capturedMetadataAfter, true);
        }

        [TestMethod]
        public void That_NothingWillBreak_When_DecoratorThrows()
        {
            var value1 = "Empty";
            var value2 = 3;
            var readonlyDecorator = new Mock<IObjectDecoratorReadonly>();
            var originalObject = new Mock<ITester>();
            readonlyDecorator
                .Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadata>()))
                .Throws(new Exception());
            readonlyDecorator
                .Setup(x => x.ExecuteAfterResult(It.IsAny<MethodMetadata>()))
                .Throws(new Exception());
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(readonlyDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            
            decorated.Method(value1, value2);
        }
        
        [TestMethod]
        public void That_ExceptionIsNotWrapped_When_BaseObjectThrows()
        {
            var value1 = "Empty";
            var value2 = 3;
            var message = "ValidateThis";
            var readonlyDecorator = new Mock<IObjectDecoratorReadonly>();
            var originalObject = new Mock<ITester>();
            originalObject
                .Setup(x => x.Method(It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception(message));
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(readonlyDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            
            Assert.ThrowsException<Exception>(() => decorated.Method(value1, value2), message);
        }
        
        [TestMethod]
        public void That_InputValuesCanBeChanged_When_SetInExecuteBeforeInvoke()
        {
            var value1 = "Empty";
            var value2 = 3;
            var volatileDecorator = new Mock<IObjectDecoratorVolatile>();
            var originalObject = new Mock<ITester>();
            volatileDecorator
                .Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>()))
                .Callback<MethodMetadataVolatile>((metadata) =>
                {
                    metadata.ParametersMetadataVolatile.SetParameters(new object[] {value1, value2});
                });
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(volatileDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            
            decorated.Method(null, -1);
            originalObject.Verify(x => x.Method(value1, value2), Times.Once);
        }
        
        [TestMethod]
        public void That_OutputCanBeChanged_When_SetInExecuteBeforeInvoke_WithoutInvokingBaseMethod()
        {
            var value2 = 3;
            var volatileDecorator = new Mock<IObjectDecoratorVolatile>();
            var originalObject = new Mock<ITester>();
            volatileDecorator
                .Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>()))
                .Callback<MethodMetadataVolatile>((metadata) =>
                {
                    metadata.ReturnMetadataVolatile.SetResult(value2);
                });
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(volatileDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            
            Assert.AreEqual(value2, decorated.ReturnMethod());
            originalObject.Verify(x => x.ReturnMethod(), Times.Never);
        }
        
        [TestMethod]
        public void That_ExceptionCanBeRaised_When_SetInExecuteBeforeInvoke_WithoutInvokingBaseMethod()
        {
            var message = "ValidateThis";
            var volatileDecorator = new Mock<IObjectDecoratorVolatile>();
            var originalObject = new Mock<ITester>();
            volatileDecorator
                .Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>()))
                .Callback<MethodMetadataVolatile>((metadata) =>
                {
                    metadata.ReturnMetadataVolatile.SetException(new Exception(message));
                });
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(volatileDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            
            Assert.ThrowsException<Exception>(() => decorated.ReturnMethod(), message);
            originalObject.Verify(x => x.ReturnMethod(), Times.Never);
        }
        
        [TestMethod]
        public void That_OutputValueCanBeChanged_SetInExecuteAfterResult()
        {
            var value2 = 3;
            var volatileDecorator = new Mock<IObjectDecoratorVolatile>();
            var originalObject = new Mock<ITester>();
            volatileDecorator
                .Setup(x => x.ExecuteAfterResult(It.IsAny<MethodMetadataVolatile>()))
                .Callback<MethodMetadataVolatile>((metadata) =>
                {
                    metadata.ReturnMetadataVolatile.SetResult(value2);
                });
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(volatileDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            
            Assert.AreEqual(value2, decorated.ReturnMethod());
            originalObject.Verify(x => x.ReturnMethod(), Times.Once);
        }
        
        [TestMethod]
        public void That_ExceptionCanBeRaised_When_SetInExecuteAfterResult()
        {
            var message = "ValidateThis";
            var volatileDecorator = new Mock<IObjectDecoratorVolatile>();
            var originalObject = new Mock<ITester>();
            volatileDecorator
                .Setup(x => x.ExecuteAfterResult(It.IsAny<MethodMetadataVolatile>()))
                .Callback<MethodMetadataVolatile>((metadata) =>
                {
                    metadata.ReturnMetadataVolatile.SetException(new Exception(message));
                });
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(volatileDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            
            Assert.ThrowsException<Exception>(() => decorated.ReturnMethod(), message);
            originalObject.Verify(x => x.ReturnMethod(), Times.Once);
        }
        
        [TestMethod]
        public void That_InputChange_When_SetInExecuteBeforeInvoke_WillEditOriginalInput_IfReference()
        {
            var value1 = 3;
            var value2 = "Edited";
            var volatileDecorator = new Mock<IObjectDecoratorVolatile>();
            var originalObject = new Mock<ITester>();
            volatileDecorator
                .Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>()))
                .Callback<MethodMetadataVolatile>((metadata) =>
                {
                    metadata.ParametersMetadataVolatile.SetParameters(new object[] {new ReferenceParams {A = value1, B = value2}});
                });
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(volatileDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();

            var input = new ReferenceParams() {A = -1, B = null};
            decorated.ReferenceMethod(input);
            Assert.AreEqual(value1, input.A);
            Assert.AreEqual(value2, input.B);
        }
        
        [TestMethod]
        public void That_SettingWrongReturnType_WillNotThrow()
        {
            var volatileDecorator = new Mock<IObjectDecoratorVolatile>();
            var originalObject = new Mock<ITester>();
            volatileDecorator
                .Setup(x => x.ExecuteBeforeInvoke(It.IsAny<MethodMetadataVolatile>()))
                .Callback<MethodMetadataVolatile>((metadata) =>
                {
                    metadata.ReturnMetadataVolatile.SetResult("wrong type");
                });
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(volatileDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();

            decorated.ReturnMethod();
        }
        
        [TestMethod]
        public void That_CanSetOutput_When_BaseObjectThrows()
        {
            var value2 = 3;
            var volatileDecorator = new Mock<IObjectDecoratorVolatile>();
            var originalObject = new Mock<ITester>();
            originalObject
                .Setup(x => x.ReturnMethod())
                .Throws(new Exception());
            
            volatileDecorator
                .Setup(x => x.ExecuteAfterResult(It.IsAny<MethodMetadataVolatile>()))
                .Callback<MethodMetadataVolatile>((metadata) =>
                {
                    metadata.ReturnMetadataVolatile.SetResult(value2);
                });
            
            var proxyDecorator = ProxyDecoratorFactory.CreateProxyDecorator<ITester>();
            proxyDecorator.SetOriginalObject(originalObject.Object);
            proxyDecorator.SetDecorator(volatileDecorator.Object);
            var decorated = proxyDecorator.GetDecoratedObject();
            
            Assert.AreEqual(value2, decorated.ReturnMethod());
        }

        public interface ITester
        {
            int Method(string value, int data);
            int ReturnMethod();
            int ReferenceMethod(ReferenceParams refParams);
        }

        public class ReferenceParams
        {
            public int A { get; set; }
            public string B { get; set; }
        }
    }
}