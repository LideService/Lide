using Lide.AsyncProxy.Tests.Proxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.AsyncProxy.Tests
{
    [TestClass]
    public class TestsUnsupportedType
    {
        [TestMethod, Ignore("Test is good, proxy is broken")]
        public void That_OutParam_CanBeUsedInProxy()
        {
            var proxyData = Helpers.GetProxy<ITesterUnsupportedOutParam, TesterUnsupportedOutParam, DifferedFunctionProxyForTests>();
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

        [TestMethod, Ignore("Test is good, proxy is broken")]
        public void That_RefParam_CanBeUsedInProxy()
        {
            var proxyData = Helpers.GetProxy<ITesterUnsupportedRefParam, TesterUnsupportedRefParam, DifferedFunctionProxyForTests>();
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

        [TestMethod]//, Ignore("Test is good, proxy is broken")]
        public void That_RefReturn_CanBeUsedInProxy()
        {
            var proxyData = Helpers.GetProxy<ITesterUnsupportedRefReturn, TesterUnsupportedRefReturn, DifferedFunctionProxyForTests>();
            var targetProxy = proxyData.TargetProxy;
            var isCalled = false;
            proxyData.SourceProxy.CallOnInvoke = (methodInfo, inputParameters) =>
            {
                isCalled = true;
                return methodInfo.Invoke(proxyData.BaseObject, inputParameters);
            };

            var result = targetProxy.Method2();
            Assert.IsTrue(isCalled);
            Assert.AreEqual("NotWorking", result);
        }

        private interface ITesterUnsupportedOutParam
        {
            void Method(out string data1);
        }

        private interface ITesterUnsupportedRefParam
        {
            void Method(ref string data1);
        }

        private interface ITesterUnsupportedRefReturn
        {
            ref string Method();
            string Method2();
        }

        private class TesterUnsupportedOutParam : ITesterUnsupportedOutParam
        {
            public void Method(out string data1)
            {
                data1 = "NotWorking";
            }
        }

        private class TesterUnsupportedRefParam : ITesterUnsupportedRefParam
        {
            public void Method(ref string data1)
            {
                if (data1 != null)
                {
                    data1 = "NotWorking";
                }
            }
        }

        private class TesterUnsupportedRefReturn : ITesterUnsupportedRefReturn
        {
            private string _value = "NotWorking";
            public ref string Method()
            {
                return ref _value;
            }
            public string Method2()
            {
                return _value;
            }
        }
    }
}