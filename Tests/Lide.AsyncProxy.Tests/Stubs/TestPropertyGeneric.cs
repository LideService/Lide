namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestPropertyGeneric<TType>
    {
        public TType ValueProperty { get; set; }
    }
    
    public class TestPropertyGeneric<TType> : ITestPropertyGeneric<TType>
    {        
        public TType ValueProperty { get; set; }
    }
}