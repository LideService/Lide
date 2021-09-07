namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestEventGeneric<TType>
    {
        delegate TType TestHandler(TType input);
        event TestHandler Event1;
        TType RaiseEvent1(TType data);
    }

    public class TestEventGeneric<TType> : ITestEventGeneric<TType>
        where TType : class
    {
        public event ITestEventGeneric<TType>.TestHandler Event1;

        public TType RaiseEvent1(TType data)
        {
            return Event1?.Invoke(data) ?? null;
        }
    }
}