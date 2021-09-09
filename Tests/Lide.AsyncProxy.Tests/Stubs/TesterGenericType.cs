namespace Lide.AsyncProxy.Tests.Stubs
{
    public class TesterGenericType<TType> : ITesterGenericType<TType>
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
            return Event?.Invoke(data) ?? null;
        }

        public TType Method(TType data)
        {
            return data;
        }
    }
}