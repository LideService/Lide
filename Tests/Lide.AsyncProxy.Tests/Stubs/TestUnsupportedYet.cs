namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestUnsupportedOutParam
    {
        void Method(out string data1);
    }

    public interface ITestUnsupportedRefParam
    {
        void Method(ref string data1);
    }
    
    public interface ITestUnsupportedRefReturn
    {
        ref string Method();
    }

    public class TestUnsupportedOutParam : ITestUnsupportedOutParam
    {
        public void Method(out string data1)
        {
            data1 = "NotWorking";
        }
    }

    public class TestUnsupportedRefParam : ITestUnsupportedRefParam
    {
        public void Method(ref string data1)
        {
            data1 = "NotWorking";
        }
    }

    public class TestUnsupportedRefReturn : ITestUnsupportedRefReturn
    {
        private string _value = "NotWorking";
        public ref string Method()
        {
            return ref _value;
        }
    }
}