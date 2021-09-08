using System;
using System.Threading.Tasks;
using Lide.AsyncProxy.Tests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.AsyncProxy.Tests
{
    [TestClass]
    public class TestsAsyncType
    {
        [TestMethod]
        public async Task That_AsyncMethods_AreProxied()
        {
            var proxyData = Helpers.GetProxy<ITestAsyncType, TestAsyncType, DefferedFunctionProxy>();
            var targetProxy = proxyData.TargetProxy;
            var sourceProxy = proxyData.SourceProxy;
            var handlerIsCalled = false;
            var handlerAsyncIsCalled = false;
            var handlerAsyncTIsCalled = false;
            sourceProxy.CallOnInvoke = (info, objects) =>
            {
                handlerIsCalled = true;
                return info.Invoke(proxyData.BaseObject, objects);
            };
            
            sourceProxy.CallOnInvokeAsync = (info, objects) =>
            {
                handlerAsyncIsCalled = true;
                return (Task)info.Invoke(proxyData.BaseObject, objects);
            };
            sourceProxy.SetCallOnInvokeAsyncT<Poco>((info, objects) =>
            {
                handlerAsyncTIsCalled = true;
                return (Task<Poco>)info.Invoke(proxyData.BaseObject, objects);
            });

            targetProxy.Method1();
            AssertAndReset(false, ref handlerIsCalled);
            AssertAndReset(true, ref handlerAsyncIsCalled);
            AssertAndReset(false, ref handlerAsyncTIsCalled);
            targetProxy.Method2(new Poco());
            AssertAndReset(false, ref handlerIsCalled);
            AssertAndReset(false, ref handlerAsyncIsCalled);
            AssertAndReset(true, ref handlerAsyncTIsCalled);
            
            await targetProxy.AsyncMethod1();
            AssertAndReset(false, ref handlerIsCalled);
            AssertAndReset(true, ref handlerAsyncIsCalled);
            AssertAndReset(false, ref handlerAsyncTIsCalled);
            await targetProxy.AsyncMethod2(new Poco());
            AssertAndReset(false, ref handlerIsCalled);
            AssertAndReset(false, ref handlerAsyncIsCalled);
            AssertAndReset(true, ref handlerAsyncTIsCalled);
        }

        private static void AssertAndReset(bool expected, ref bool value)
        {
            Assert.AreEqual(expected, value);
            value = false;
        }
    }
}