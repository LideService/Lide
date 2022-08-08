using System;
using System.Reflection;
using System.Threading.Tasks;
using Lide.AsyncProxy.Tests.Proxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.AsyncProxy.Tests
{
    [TestClass]
    public class TestGeneratedTypes
    {

        [TestMethod]
        public void That_MultipleInstances_HaveTheSameType()
        {
            var proxyData1 = DispatchProxyAsyncFactory.Create(typeof(ITester), typeof(BaseObjectProxy));
            var proxyData2 = DispatchProxyAsyncFactory.Create(typeof(ITester), typeof(BaseObjectProxy));
            var proxyData3 = DispatchProxyAsyncFactory.Create(typeof(ITester), typeof(BaseDispatchProxy));

            ((BaseObjectProxy)proxyData1).BaseObject = new Tester();
            ((BaseObjectProxy)proxyData2).BaseObject = new Tester();
            ((BaseDispatchProxy)proxyData3).CallOnInvoke = (methodInfo, objParams) => null; 
            
            ((ITester)proxyData1).Do();
            ((ITester)proxyData2).Do();
            ((ITester)proxyData3).Do();
            Assert.AreNotEqual(proxyData1, proxyData2);
            Assert.AreNotEqual(proxyData1, proxyData3);
            Assert.AreEqual(proxyData1.GetType(), proxyData2.GetType());
            Assert.AreNotEqual(proxyData1.GetType(), proxyData3.GetType());
        }

        private interface ITester
        {
            void Do();
        }

        private class Tester : ITester
        {
            public void Do()
            {
            }
        }
    }
}