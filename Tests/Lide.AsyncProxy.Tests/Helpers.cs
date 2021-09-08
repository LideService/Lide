using System;
using Lide.AsyncProxy.Tests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.AsyncProxy.Tests
{
    public static class Helpers
    {
        public class ProxyData<TInterface, TImplementation, TProxy>
            where TImplementation : TInterface, new()
        {
            public ProxyData(TImplementation baseObject, TInterface targetProxy, TProxy sourceProxy)
            {
                BaseObject = baseObject;
                TargetProxy = targetProxy;
                SourceProxy = sourceProxy;
            }

            public TImplementation BaseObject { get; }
            public TInterface TargetProxy { get; }
            public TProxy SourceProxy { get; }
        }

        public static ProxyData<TInterface, TImpl, TProxy> GetProxy<TInterface, TImpl, TProxy>()
            where TImpl : TInterface, new()
        {
            var baseObject = new TImpl();
            var proxy = DispatchProxyAsyncFactory.Create(typeof(TInterface), typeof(TProxy));
            var targetProxy = (TInterface)proxy;
            var sourceProxy = (TProxy)proxy;

            Assert.IsNotNull(proxy);
            Assert.IsInstanceOfType(proxy, typeof(TInterface));
            Assert.IsInstanceOfType(proxy, typeof(TProxy));

            return new ProxyData<TInterface, TImpl, TProxy>(baseObject, targetProxy, sourceProxy);
        }

        public static void AssertExecuteOnInvoke<TInterface, TImpl, TProxy>(ProxyData<TInterface, TImpl, TProxy> proxyData, Action action, object expectedOutput, params object[] expectedInput)
            where TImpl : class, TInterface, new()
            where TProxy : class, ICallOnInvoke
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