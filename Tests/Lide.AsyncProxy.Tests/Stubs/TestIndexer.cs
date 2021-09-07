namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestIndexer
    {
        long this [long index] { get; set; }
        int this [int index] { get; }
        string this [string index] { set; }
    }

    public class TestIndexer : ITestIndexer
    {
        private long _longIndexer;
        private int _intIndexer;
        
        public long this[long index]
        {
            get => _longIndexer;
            set => _longIndexer = value;
        }

        public int this[int index] => _intIndexer;

        public string this[string index]
        {
            set => _intIndexer = value.Length;
        }
    }
}