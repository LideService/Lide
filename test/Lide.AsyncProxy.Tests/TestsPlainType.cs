/* cSpell:disable */
using System;
using System.Runtime.ExceptionServices;
using Lide.AsyncProxy.Tests.Proxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.AsyncProxy.Tests
{
    [TestClass]
    public class TestsPlainType
    {
        [TestMethod]
        public void That_Properties_AreProxied()
        {
            var proxyData = Helpers.GetProxy<ITesterPlainType, TesterPlainType, DifferedFunctionProxyForTests>();
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITesterInheritedBase1));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITesterInheritedBase2));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITesterInheritedBase3));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITesterPlainType));

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
            var proxyData = Helpers.GetProxy<ITesterPlainType, TesterPlainType, DifferedFunctionProxyForTests>();
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
            var proxyData = Helpers.GetProxy<ITesterPlainType, TesterPlainType, DifferedFunctionProxyForTests>();
            var targetProxy = proxyData.TargetProxy;
            var handlerIsCalled = false;
            var poco = new Poco { StringField = "Poco3", IntField = 7 };
            void Handler1() => handlerIsCalled = true;
            int Handler2(int data) { handlerIsCalled = true; return data; }
            Poco Handler3(Poco data) { handlerIsCalled = true; return data; }
            void AssertHandler(bool isCalled = true) { Assert.AreEqual(isCalled, handlerIsCalled); handlerIsCalled = false; }

            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 += Handler1, null, (TesterHandler)Handler1);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event2 += Handler2, null, (TesterHandlerValue)Handler2);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event3 += Handler3, null, (TesterHandlerReference)Handler3);

            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent1(); AssertHandler(); }, null);
            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent2(13); AssertHandler(); }, 13, 13);
            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent3(poco); AssertHandler(); }, poco, poco);

            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 -= Handler1, null, (TesterHandler)Handler1);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event2 -= Handler2, null, (TesterHandlerValue)Handler2);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event3 -= Handler3, null, (TesterHandlerReference)Handler3);

            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent1(); AssertHandler(false); }, null);
            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent2(13); AssertHandler(false); }, -1, 13);
            Helpers.AssertExecuteOnInvoke(proxyData, () => { targetProxy.RaiseEvent3(poco); AssertHandler(false); }, null, poco);
        }

        [TestMethod]
        public void That_Methods_AreProxied()
        {
            var proxyData = Helpers.GetProxy<ITesterPlainType, TesterPlainType, DifferedFunctionProxyForTests>();
            var targetProxy = proxyData.TargetProxy;
            var poco = new Poco { StringField = "Poco4", IntField = 9 };
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.PlainMethod(9, poco), poco, 9, poco);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.ParamsMethod(7, 8, 9, 1, 2, 3), 12, 7, new object[] { 8, 9, 1, 2, 3 });
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.OptionalMethod(5, 9), 14, 5, 9);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.OptionalMethod(6), 5, 6, -1);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.ImplementedMethod(13, 8), 21, 13, 8);
        }

        [TestMethod]
        public void That_Methods_CanThrowProperException()
        {
            var proxyData = Helpers.GetProxy<ITesterPlainType, TesterPlainType, BaseObjectProxy>();
            var targetProxy = proxyData.TargetProxy;
            proxyData.SourceProxy.BaseObject = proxyData.BaseObject;

            var exception = new Exception("Thrown");
            Assert.ThrowsException<Exception>(() => targetProxy.ThrowMethod(exception), exception.Message);
        }

        private delegate void TesterHandler();
        private delegate int TesterHandlerValue(int data);
        private delegate Poco TesterHandlerReference(Poco data);

        private interface ITesterInheritedBase1
        {
            int BaseField1 { get; set; }
        }

        private interface ITesterInheritedBase2 : ITesterInheritedBase1
        {
            int BaseField2 { get; set; }
        }

        private interface ITesterInheritedBase3
        {
            int BaseField3 { get; set; }
        }

        private interface ITesterPlainType : ITesterInheritedBase2, ITesterInheritedBase3
        {
            int IntProperty { get; set; }
            decimal DecimalProperty { get; set; }
            string StringProperty { get; set; }
            object GetterOnly { get; }
            object SetterOnly { set; }

            long this[long index] { get; set; }
            int this[int index] { get; }
            Poco this[Poco index] { set; }

            event TesterHandler Event1;
            event TesterHandlerValue Event2;
            event TesterHandlerReference Event3;
            void RaiseEvent1();
            int RaiseEvent2(int data);
            Poco RaiseEvent3(Poco data);

            Poco PlainMethod(int number, Poco data);
            int ParamsMethod(int data1, params object[] dataParams);
            int OptionalMethod(int data1, int data2 = -1);
            int ImplementedMethod(int data1, int data2)
            {
                return data1 + data2;
            }

            void ThrowMethod(Exception e);
        }

        private class TesterPlainType : ITesterPlainType
        {
            private object _getterSetterValue;
            private long _longIndexer;
            private int _intIndexer;

            #region inheritance
            public int BaseField1 { get; set; }
            public int BaseField2 { get; set; }
            public int BaseField3 { get; set; }
            #endregion

            #region properties
            public int IntProperty { get; set; }
            public decimal DecimalProperty { get; set; }
            public string StringProperty { get; set; }
            public object GetterOnly => _getterSetterValue;
            public object SetterOnly
            {
                set => _getterSetterValue = value;
            }
            #endregion

            #region indexers
            public long this[long index]
            {
                get => _longIndexer;
                set => _longIndexer = value;
            }

            public int this[int index] => _intIndexer + index;

            public Poco this[Poco index]
            {
                set => _intIndexer = index.IntField + value.IntField;
            }


            #endregion

            #region methods
            public Poco PlainMethod(int number, Poco data)
            {
                data.IntField += number;
                data.StringField = $"{data.StringField}.{number}";
                return data;
            }

            public int ParamsMethod(int data1, params object[] dataParams)
            {
                return data1 + dataParams.Length;
            }

            public int OptionalMethod(int data1, int data2 = -1)
            {
                return data1 + data2;
            }

            public void ThrowMethod(Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }
            #endregion

            #region events
            public event TesterHandler Event1;
            public event TesterHandlerValue Event2;
            public event TesterHandlerReference Event3;
            public void RaiseEvent1()
            {
                Event1?.Invoke();
            }

            public int RaiseEvent2(int data)
            {
                return Event2?.Invoke(data) ?? -1;
            }

            public Poco RaiseEvent3(Poco data)
            {
                return Event3?.Invoke(data);
            }
            #endregion
        }
    }
}