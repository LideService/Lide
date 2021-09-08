using System;
using Lide.AsyncProxy.Tests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.AsyncProxy.Tests
{
    [TestClass]
    public class TestsGenericType
    {
        [TestMethod]
        public void That_GenericType_CanBeProxied()
        {
            var proxyData = Helpers.GetProxy<ITestGenericType<Poco>, TestGenericType<Poco>, DefferedFunctionProxy>();
            var targetProxy = proxyData.TargetProxy;
            var poco = new Poco();
            var handlerIsCalled = false;
            Poco Handler(Poco input)
            {
                handlerIsCalled = true;
                return input;
            }

            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.ValueProperty = poco, null, poco);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy[poco] = poco, null, poco, poco);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event += Handler, null, (TestGenericHandler<Poco>)Handler);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.RaiseEvent(poco), poco, poco);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Method(poco), poco, poco);
            Assert.IsTrue(handlerIsCalled);
        }
    }
}