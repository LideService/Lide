namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestInheritedMultipleBaseBase
    {
        int Field1 { get; set; }
    }

    public interface ITestInheritedMultipleBase1 : ITestInheritedMultipleBaseBase
    {
        int Field2 { get; set; }
    }

    public interface ITestInheritedMultipleBase2
    {
        int Field3 { get; set; }
    }

    public interface ITestInheritedMultiple : ITestInheritedMultipleBase1, ITestInheritedMultipleBase2
    {
        int Field4 { get; set; }
    }

    public class TestInheritedMultiple : ITestInheritedMultiple
    {
        public int Field1 { get; set; }
        public int Field2 { get; set; }
        public int Field3 { get; set; }
        public int Field4 { get; set; }
    }
}