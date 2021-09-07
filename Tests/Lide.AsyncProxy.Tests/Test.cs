using System;
using Lide.AsyncProxy.Tests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITestMethod = Lide.AsyncProxy.Tests.Stubs.ITestMethod;

namespace Lide.AsyncProxy.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void That_Properties_AreProxied()
        {
            var proxyData = GetProxy<ITestProperty, TestProperty>();
            var targetProxy = proxyData.TargetProxy;
            var dataField1 = 1;
            var dataField2 = 3.4m;
            var dataField3 = "Variant test";
            var dataField4 = new object();
            AssertExecuteOnInvoke(proxyData, () => targetProxy.IntProperty = dataField1, null, dataField1);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.DecimalProperty = dataField2, null, dataField2);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.StringProperty = dataField3, null, dataField3);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.SetterOnly = dataField4, null, dataField4);

            AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(dataField1, targetProxy.IntProperty), dataField1);
            AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(dataField2, targetProxy.DecimalProperty), dataField2);
            AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(dataField3, targetProxy.StringProperty), dataField3);
            AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(dataField4, targetProxy.GetterOnly), dataField4);
        }

        [TestMethod]
        public void That_GenericProperty_IsProxied()
        {
            var proxyData = GetProxy<ITestPropertyGeneric<string>, TestPropertyGeneric<string>>();
            var targetProxy = proxyData.TargetProxy;
            var dataField1 = "Variant test or not";
            AssertExecuteOnInvoke(proxyData, () => targetProxy.ValueProperty = dataField1, null, dataField1);
            AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(dataField1, targetProxy.ValueProperty), dataField1);
        }

        [TestMethod]
        public void That_Indexers_AreProxied()
        {
            var proxyData = GetProxy<ITestIndexer, TestIndexer>();
            var targetProxy = proxyData.TargetProxy;
            var dataField1 = 1L;
            var indexField1 = 13L;
            var indexField2 = 14;
            var dataField3 = "Variant test";
            var indexField3 = "NonVariant";
            AssertExecuteOnInvoke(proxyData, () => targetProxy[indexField1] = dataField1, null, indexField1, dataField1);
            AssertExecuteOnInvoke(proxyData, () => targetProxy[indexField3] = dataField3, null, indexField3, dataField3);

            AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(dataField1, targetProxy[indexField1]), dataField1, indexField1);
            AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(dataField3.Length, targetProxy[indexField2]), dataField3.Length, indexField2);
        }

        [TestMethod]
        public void That_GenericIndexers_AreProxied()
        {
            var proxyData = GetProxy<ITestIndexerGeneric<string>, TestIndexerGeneric<string>>();
            var targetProxy = proxyData.TargetProxy;
            var dataField = "Variant test";
            var indexField = "NonVariant";
            AssertExecuteOnInvoke(proxyData, () => targetProxy[indexField] = dataField, null, indexField, dataField);
            AssertExecuteOnInvoke(proxyData, () => Assert.AreEqual(dataField, targetProxy[indexField]), dataField, indexField);
        }

        [TestMethod]
        public void That_Events_AreProxied()
        {
            var proxyData = GetProxy<ITestEvent, TestEvent>();
            var targetProxy = proxyData.TargetProxy;
            var handler1Called = false;
            var handler2Called = false;
            var handler3Called = false;
            var data = "Test stuff";
            void Handler1() => handler1Called = true;
            void Handler2() => handler2Called = true;

            int Handler3(string input)
            {
                handler3Called = true;
                return input.Length;
            }

            AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 += Handler1, null, (ITestEvent.TestHandler)Handler1);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 += Handler2, null, (ITestEvent.TestHandler)Handler2);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Event2 += Handler3, null, (ITestEvent.TestHandlerWithParam)Handler3);

            AssertExecuteOnInvoke(proxyData, () => targetProxy.RaiseEvent1(), null);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.RaiseEvent2(data), data.Length, data);

            Assert.IsTrue(handler1Called);
            Assert.IsTrue(handler2Called);
            Assert.IsTrue(handler3Called);

            handler1Called = false;
            handler2Called = false;
            handler3Called = false;
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 -= Handler1, null, (ITestEvent.TestHandler)Handler1);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 -= Handler2, null, (ITestEvent.TestHandler)Handler2);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Event2 -= Handler3, null, (ITestEvent.TestHandlerWithParam)Handler3);

            AssertExecuteOnInvoke(proxyData, () => targetProxy.RaiseEvent1(), null);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.RaiseEvent2(data), -1, data);
            Assert.IsFalse(handler1Called);
            Assert.IsFalse(handler2Called);
            Assert.IsFalse(handler3Called);
        }


        [TestMethod]
        public void That_GenericEvents_AreProxied()
        {
            var proxyData = GetProxy<ITestEventGeneric<string>, TestEventGeneric<string>>();
            var targetProxy = proxyData.TargetProxy;
            var handler1Called = false;
            var data = "Test stuff";
            var output = "Return stuff";

            string Handler1(string input)
            {
                handler1Called = true;
                return $"{input}.{output}";
            }

            AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 += Handler1, null, (ITestEventGeneric<string>.TestHandler)Handler1);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.RaiseEvent1(data), $"{data}.{output}", data);
            Assert.IsTrue(handler1Called);

            handler1Called = false;
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Event1 -= Handler1, null, (ITestEventGeneric<string>.TestHandler)Handler1);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.RaiseEvent1(data), null, data);
            Assert.IsFalse(handler1Called);
        }

        [TestMethod]
        public void That_Inheritance_IsProxied()
        {
            var proxyData = GetProxy<ITestInheritedOnce, TestInheritedOnce>();
            var targetProxy = proxyData.TargetProxy;
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestInheritedOnce));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestInheritedOnceBase));

            AssertExecuteOnInvoke(proxyData, () => targetProxy.Field1 = 1, null, 1);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Field2 = 2, null, 2);
        }

        [TestMethod]
        public void That_MultipleInheritance_AreProxied()
        {
            var proxyData = GetProxy<ITestInheritedMultiple, TestInheritedMultiple>();
            var targetProxy = proxyData.TargetProxy;
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestInheritedMultiple));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestInheritedMultipleBase1));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestInheritedMultipleBase2));
            Assert.IsInstanceOfType(proxyData.SourceProxy, typeof(ITestInheritedMultipleBaseBase));

            AssertExecuteOnInvoke(proxyData, () => targetProxy.Field1 = 1, null, 1);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Field2 = 2, null, 2);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Field3 = 3, null, 3);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.Field4 = 4, null, 4);
        }

        [TestMethod]
        public void That_Methods_AreProxied()
        {
            var proxyData = GetProxy<ITestMethod, TestMethod>();
            var targetProxy = proxyData.TargetProxy;
            var data1 = 4;
            var data2 = 7;
            var data3 = "TestData";
            AssertExecuteOnInvoke(proxyData, () => targetProxy.PlainMethod1(), null);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.PlainMethod2(data1, data2), null, data1, data2);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.PlainMethod3(data3), $"{data3}.{data3}", data3);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.ParamsMethod(data1, data2, data3), data1 + 2, data1, new object[] { data2, data3 });
            AssertExecuteOnInvoke(proxyData, () => targetProxy.OptionalMethod(data1, data2), data1 + data2, data1, data2);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.OptionalMethod(data1), data1 - 1, data1, -1);
            AssertExecuteOnInvoke(proxyData, () => targetProxy.ImplementedMethod(data1, data2), data1 * 3 + data2 * 4, data1, data2);
        }

        [TestMethod]
        public void That_Methods_CanThrowProperException()
        {
            var baseObject = new TestMethod();
            var proxy = DispatchProxyAsyncFactory.Create(typeof(ITestMethod), typeof(DefaultProxyForException));
            var targetProxy2 = (ITestMethod)proxy;
            var sourceProxy2 = (DefaultProxyForException)proxy;
            sourceProxy2.OriginalObject = baseObject;

            var exception = new Exception("Thrown");
            Assert.ThrowsException<Exception>(() => targetProxy2.ThrowMethod(exception), exception.Message);
        }

        [TestMethod]
        public void That_Unsupported_ArentWorkingYet()
        {
            try
            {
                var baseObject = new TestUnsupported();
                var proxy = DispatchProxyAsyncFactory.Create(typeof(ITestUnsupported), typeof(DefaultProxy));
                var targetProxy = (ITestUnsupported)proxy;
                var sourceProxy = (DefaultProxy)proxy;
                sourceProxy.CallOnInvoke = (methodInfo, inputParameters) => methodInfo.Invoke(baseObject, inputParameters);

                string inputFailure = null;
                string returnFailure = null;
                targetProxy.OutParam(out var failure1);
                targetProxy.RefParam(ref inputFailure);
                returnFailure = targetProxy.RefReturn(ref inputFailure);
                Assert.Fail();
            }
            catch
            {
                
            }
        }

        private class ProxyData<TInterface, TImplementation>
            where TImplementation : TInterface, new()
        {
            public ProxyData(TImplementation baseObject, TInterface targetProxy, DefaultProxy sourceProxy)
            {
                BaseObject = baseObject;
                TargetProxy = targetProxy;
                SourceProxy = sourceProxy;
            }

            public TImplementation BaseObject { get; }
            public TInterface TargetProxy { get; }
            public DefaultProxy SourceProxy { get; }
        }

        private static ProxyData<TInterface, TImpl> GetProxy<TInterface, TImpl>()
            where TImpl : TInterface, new()
        {
            var baseObject = new TImpl();
            var proxy = DispatchProxyAsyncFactory.Create(typeof(TInterface), typeof(DefaultProxy));
            var targetProxy = (TInterface)proxy;
            var sourceProxy = (DefaultProxy)proxy;

            Assert.IsNotNull(proxy);
            Assert.IsInstanceOfType(proxy, typeof(TInterface));
            Assert.IsInstanceOfType(proxy, typeof(DefaultProxy));

            return new ProxyData<TInterface, TImpl>(baseObject, targetProxy, sourceProxy);
        }

        private static void AssertExecuteOnInvoke<TInterface, TImpl>(ProxyData<TInterface, TImpl> proxyData, Action action, object expectedOutput, params object[] expectedInput)
            where TImpl : TInterface, new()
        {
            var isCalled = false;
            object[] actualInput = null;
            object actualOutput = null;
            proxyData.SourceProxy.CallOnInvoke = (methodInfo, inputParameters) =>
            {
                isCalled = true;
                actualInput = inputParameters;
                actualOutput = methodInfo.Invoke(proxyData.BaseObject, inputParameters);
                return actualOutput;
            };

            action?.Invoke();

            Assert.IsTrue(isCalled);
            Assert.AreEqual(expectedOutput, actualOutput);
            AssertArrays(expectedInput, actualInput);
        }

        private static void AssertArrays(object[] expectedInput, object[] actualInput)
        {
            Assert.AreEqual(expectedInput.Length, actualInput.Length);
            for (var i = 0; i < actualInput.Length; i++)
            {
                if (expectedInput[i].GetType().IsArray)
                {
                    AssertArrays((object[])expectedInput[i], (object[])actualInput[i]);
                    continue;
                }

                Assert.AreEqual(expectedInput[i], actualInput[i]);
            }
        }
    }
}