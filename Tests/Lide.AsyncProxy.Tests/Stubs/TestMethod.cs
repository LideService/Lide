using System;
using System.Runtime.ExceptionServices;

namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestMethod
    {
        void PlainMethod1();
        void PlainMethod2(int data1, int data2);
        string PlainMethod3(string data);
        int ParamsMethod(int data1, params object[] dataParams);
        int OptionalMethod(int data1, int data2 = -1);
        void ThrowMethod(Exception e);

        int ImplementedMethod(int data1, int data2)
        {
            return data1 * 3 + data2 * 4;
        }
    }
    
    public class TestMethod : ITestMethod
    {
        public void PlainMethod1()
        {
        }

        public void PlainMethod2(int data1, int data2)
        {
        }

        public string PlainMethod3(string data)
        {
            return $"{data}.{data}";
        }

        public int ParamsMethod(int data1, params object[] dataParams)
        {
            return data1 + dataParams.Length;
        }

        public int OptionalMethod(int data1, int data2 = -1)
        {
            return data1 + data2;
        }

        public void ThrowMethod(Exception e)
        {
            ExceptionDispatchInfo.Capture(e).Throw();
        }
    }
}