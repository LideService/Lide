using Lide.AsyncProxy.Tests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.AsyncProxy.Tests
{
    [TestClass]
    public class TestsUnsupportedType
    {
        [TestMethod, Ignore("Test is good, testee is broken")]
        public void That_OutParam_IsProxied()
        {
            var proxyData = Helpers.GetProxy<ITesterUnsupportedOutParam, TesterUnsupportedOutParam, DefferedFunctionProxy>();
            var targetProxy = proxyData.TargetProxy;
            var isCalled = false;
            proxyData.SourceProxy.CallOnInvoke = (methodInfo, inputParameters) =>
            {
                isCalled = true;
                return methodInfo.Invoke(proxyData.BaseObject, inputParameters);
            };

            targetProxy.Method(out var result);
            Assert.IsTrue(isCalled);
            Assert.AreEqual("NotWorking", result);
        }
        
        [TestMethod]
        public void That_OutParam_IsNotProxied_WithDispatchProxy()
        {
            var proxyData = Helpers.GetProxy<ITesterUnsupportedOutParam, TesterUnsupportedOutParam, BaseDispatchProxy>();
            var targetProxy = proxyData.TargetProxy;
            var isCalled = false;
            proxyData.SourceProxy.CallOnInvoke = (methodInfo, inputParameters) =>
            {
                isCalled = true;
                return methodInfo.Invoke(proxyData.BaseObject, inputParameters);
            };

            try
            {

                targetProxy.Method(out var result);
                Assert.IsTrue(isCalled);
                Assert.AreEqual("NotWorking", result);
                Assert.Fail();
            }
            catch
            {
                // ignored
            }
        }
        [TestMethod, Ignore("Test is good, testee is broken")]
        public void That_RefParam_IsProxied()
        {
            var proxyData = Helpers.GetProxy<ITesterUnsupportedRefParam, TesterUnsupportedRefParam, DefferedFunctionProxy>();
            var targetProxy = proxyData.TargetProxy;
            var isCalled = false;
            proxyData.SourceProxy.CallOnInvoke = (methodInfo, inputParameters) =>
            {
                isCalled = true;
                return methodInfo.Invoke(proxyData.BaseObject, inputParameters);
            };

            var result = "";
            targetProxy.Method(ref result);
            Assert.IsTrue(isCalled);
            Assert.AreEqual("NotWorking", result);
        }
        
        [TestMethod]
        public void That_RefParam_IsNotProxied_WithDispatchProxy()
        {
            var proxyData = Helpers.GetProxy<ITesterUnsupportedRefParam, TesterUnsupportedRefParam, BaseDispatchProxy>();
            var targetProxy = proxyData.TargetProxy;
            var isCalled = false;
            proxyData.SourceProxy.CallOnInvoke = (methodInfo, inputParameters) =>
            {
                isCalled = true;
                return methodInfo.Invoke(proxyData.BaseObject, inputParameters);
            };

            try
            {
                var result = "";
                targetProxy.Method(ref result);
                Assert.IsTrue(isCalled);
                Assert.AreEqual("NotWorking", result);
                Assert.Fail();
            }
            catch
            {
                // ignored
            }
        }
        [TestMethod, Ignore("Test is good, testee is broken")]
        public void That_RefReturn_IsProxied()
        {
            var proxyData = Helpers.GetProxy<ITesterUnsupportedRefReturn, TesterUnsupportedRefReturn, DefferedFunctionProxy>();
            var targetProxy = proxyData.TargetProxy;
            var isCalled = false;
            proxyData.SourceProxy.CallOnInvoke = (methodInfo, inputParameters) =>
            {
                isCalled = true;
                return methodInfo.Invoke(proxyData.BaseObject, inputParameters);
            };

            var result = targetProxy.Method();
            Assert.IsTrue(isCalled);
            Assert.AreEqual("NotWorking", result);
        }
        
        [TestMethod]
        public void That_RefResult_IsNotProxied_WithDispatchProxy()
        {
            try
            {
                var proxyData = Helpers.GetProxy<ITesterUnsupportedRefReturn, TesterUnsupportedRefReturn, BaseDispatchProxy>();
                var targetProxy = proxyData.TargetProxy;
                var isCalled = false;
                proxyData.SourceProxy.CallOnInvoke = (methodInfo, inputParameters) =>
                {
                    isCalled = true;
                    return methodInfo.Invoke(proxyData.BaseObject, inputParameters);
                };
                var result = targetProxy.Method();
                Assert.IsTrue(isCalled);
                Assert.AreEqual("NotWorking", result);
                Assert.Fail();
            }
            catch
            {
                // ignored
            }
        }
    }
}