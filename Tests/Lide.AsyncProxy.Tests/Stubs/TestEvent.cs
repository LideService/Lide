namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestEvent
    {
        delegate void TestHandler();
        delegate int TestHandlerWithParam(string data);
        event TestHandler Event1;
        event TestHandlerWithParam Event2;
        void RaiseEvent1();
        int RaiseEvent2(string data);
    }

    public class TestEvent : ITestEvent
    {
        public event ITestEvent.TestHandler Event1;
        public event ITestEvent.TestHandlerWithParam Event2;

        public void RaiseEvent1()
        {
            Event1?.Invoke();
        }
        
        public int RaiseEvent2(string data)
        {
            return Event2?.Invoke(data) ?? -1;
        }
    }
}