using System;
using Lide.AsyncProxy.Tests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.AsyncProxy.Tests
{
    [TestClass]
    public class TestsPlainType
    {
        [TestMethod]
        public void That_Properties_AreProxied()
        {
            var proxyData = Helpers.GetProxy<ITestPlainType, TestPlainType, DefferedFunctionProxy>();
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestInheritedBase1));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestInheritedBase2));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestInheritedBase3));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestPlainType));
            
            var targetProxy = proxyData.TargetProxy;
            var dataField = new object();
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.BaseField1 = 17, null, 17);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.BaseField2 = 19, null, 19);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.BaseField3 = 21, null, 21);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.IntProperty = 173, null, 173);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.DecimalProperty = 29.4m, null, 29.4m);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.StringProperty = "Text to validate", null, "Text to validate");
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.SetterOnly = dataField, null, dataField);

            Helpers.AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(17, targetProxy.BaseField1), 17);
            Helpers.AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(19, targetProxy.BaseField2), 19);
            Helpers.AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(21, targetProxy.BaseField3), 21);
            Helpers.AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(173, targetProxy.IntProperty), 173);
            Helpers.AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(29.4m, targetProxy.DecimalProperty), 29.4m);
            Helpers.AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual("Text to validate", targetProxy.StringProperty), "Text to validate");
            Helpers.AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(dataField, targetProxy.GetterOnly), dataField);
        }

        [TestMethod]
        public void That_Indexers_AreProxied()
        {
            var proxyData = Helpers.GetProxy<ITestPlainType, TestPlainType, DefferedFunctionProxy>();
            var targetProxy = proxyData.TargetProxy;
            var dataFiled1 = new Poco() { StringField = "Poco1", IntField = -29 };
            var dataFiled2 = new Poco() { StringField = "Poco2", IntField = -48 };
            var expectedResult = dataFiled1.IntField + dataFiled2.IntField + 17;
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy[19L] = 23L, null, 19L, 23L);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy[dataFiled1] = dataFiled2, null, dataFiled1, dataFiled2);

            Helpers.AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(23L, targetProxy[19L]), 23L, 19L);
            Helpers.AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(expectedResult, targetProxy[17]), expectedResult, 17);
        }

        [TestMethod]
        public void That_Events_AreProxied()
        {
            var proxyData = Helpers.GetProxy<ITestPlainType, TestPlainType, DefferedFunctionProxy>();
            var targetProxy = proxyData.TargetProxy;
            var handlerIsCalled = false;
            var poco = new Poco { StringField = "Poco3", IntField = 7 };
            void Handler1() => handlerIsCalled = true;
            int Handler2(int data) { handlerIsCalled = true; return data; }
            Poco Handler3(Poco data) { handlerIsCalled = true; return data; }
            void AssertHandler(bool isCalled = true) { Assert.AreEqual(isCalled, handlerIsCalled); handlerIsCalled = false; }

            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 += Handler1, null, (TestHandler)Handler1);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event2 += Handler2, null, (TestHandlerValue)Handler2);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event3 += Handler3, null, (TestHandlerReference)Handler3);

            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent1(); AssertHandler(); }, null);
            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent2(13); AssertHandler(); }, 13, 13);
            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent3(poco); AssertHandler(); }, poco, poco);

            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 -= Handler1, null, (TestHandler)Handler1);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event2 -= Handler2, null, (TestHandlerValue)Handler2);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event3 -= Handler3, null, (TestHandlerReference)Handler3);

            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent1(); AssertHandler(false); }, null);
            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent2(13); AssertHandler(false); }, -1, 13);
            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent3(poco); AssertHandler(false); }, null, poco);
        }
        
        [TestMethod]
        public void That_Methods_AreProxied()
        {
            var proxyData = Helpers.GetProxy<ITestPlainType, TestPlainType, DefferedFunctionProxy>();
            var targetProxy = proxyData.TargetProxy;
            var poco = new Poco { StringField = "Poco4", IntField = 9 };
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.PlainMethod(9, poco), poco, 9 , poco);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.ParamsMethod(7, 8, 9 , 1, 2 ,3), 12, 7, new object[] {8,9,1,2,3});
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.OptionalMethod(5, 9), 14, 5, 9);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.OptionalMethod(6), 5, 6, -1);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.ImplementedMethod(13, 8), 21, 13, 8);
        }

        [TestMethod]
        public void That_Methods_CanThrowProperException()
        {
            var proxyData = Helpers.GetProxy<ITestPlainType, TestPlainType, BaseObjectProxy>();
            var targetProxy = proxyData.TargetProxy;
            proxyData.SourceProxy.BaseObject = proxyData.BaseObject;

            var exception = new Exception("Thrown");
            Assert.ThrowsException<Exception>(() => targetProxy.ThrowMethod(exception), exception.Message);
        }
    }
}