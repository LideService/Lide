namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestUnsupported
    {
        void OutParam(out string data1);
        void RefParam(ref string data1);
        ref string RefReturn(ref string data1);
    }
    
    public class TestUnsupported : ITestUnsupported
    {
        public void OutParam(out string data1)
        {
            data1 = "NotWorking";
        }

        public void RefParam(ref string data1)
        {
            data1 = "NotWorking";
        }

        public ref string RefReturn(ref string data1)
        {
            return ref data1;
        }
    }
}