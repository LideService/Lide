namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestInheritedOnceBase
    {
        int Field1 { get; set; }    
    }
    
    public interface ITestInheritedOnce : ITestInheritedOnceBase
    {
        int Field2 { get; set; }    
    }

    public class TestInheritedOnce : ITestInheritedOnce
    {
        public int Field1 { get; set; }
        public int Field2 { get; set; }
    }
}