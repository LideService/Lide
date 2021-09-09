namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITesterUnsupportedOutParam
    {
        void Method(out string data1);
    }

    public interface ITesterUnsupportedRefParam
    {
        void Method(ref string data1);
    }
    
    public interface ITesterUnsupportedRefReturn
    {
        ref string Method();
    }

    public class TesterUnsupportedOutParam : ITesterUnsupportedOutParam
    {
        public void Method(out string data1)
        {
            data1 = "NotWorking";
        }
    }

    public class TesterUnsupportedRefParam : ITesterUnsupportedRefParam
    {
        public void Method(ref string data1)
        {
            data1 = "NotWorking";
        }
    }

    public class TesterUnsupportedRefReturn : ITesterUnsupportedRefReturn
    {
        private string _value = "NotWorking";
        public ref string Method()
        {
            return ref _value;
        }
    }
}