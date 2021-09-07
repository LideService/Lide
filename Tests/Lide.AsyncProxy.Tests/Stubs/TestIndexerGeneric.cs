namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestIndexerGeneric<TType>
    {
        TType this [TType index] { get; set; }
    }

    public class TestIndexerGeneric<TType> : ITestIndexerGeneric<TType>
    {
        private TType _indexer;
        
        public TType this [TType index]
        {
            get => _indexer;
            set => _indexer = value;
        }
    }
}