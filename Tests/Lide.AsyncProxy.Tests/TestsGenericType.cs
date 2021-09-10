using Lide.AsyncProxy.Tests.Proxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.AsyncProxy.Tests
{
    [TestClass]
    public class TestsGenericType
    {
        [TestMethod]
        public void That_GenericType_CanBeProxied()
        {
            var proxyData = Helpers.GetProxy<ITesterGenericType<Poco>, TesterGenericType<Poco>, DefferedFunctionProxy>();
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
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Event += Handler, null, (TesterGenericHandler<Poco>)Handler);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.RaiseEvent(poco), poco, poco);
            Helpers.AssertExecuteOnInvoke(proxyData, () => targetProxy.Method(poco), poco, poco);
            Assert.IsTrue(handlerIsCalled);
        }

        private delegate TType TesterGenericHandler<TType>(TType input)
            where TType : class;

        private interface ITesterGenericType<TType>
            where TType : class
        {
            TType ValueProperty { get; set; }
            TType this [TType index] { get; set; }
            event TesterGenericHandler<TType> Event;
            TType RaiseEvent(TType data);
            TType Method(TType data);
        }
        
        private class TesterGenericType<TType> : ITesterGenericType<TType>
            where TType : class
        {        
            private TType _indexer;
            public TType ValueProperty { get; set; }
        
            public TType this [TType index]
            {
                get => _indexer;
                set => _indexer = value;
            }
        
            public event TesterGenericHandler<TType> Event;

            public TType RaiseEvent(TType data)
            {
                return Event?.Invoke(data);
            }

            public TType Method(TType data)
            {
                return data;
            }
        }
    }
}