using System;

namespace Lide.AsyncProxy.Tests.Stubs
{
    public delegate void TestHandler();
    public delegate int TestHandlerValue(int data);
    public delegate Poco TestHandlerReference(Poco data);
    
    public interface ITestInheritedBase1
    {
        int BaseField1 { get; set; }
    }

    public interface ITestInheritedBase2 : ITestInheritedBase1
    {
        int BaseField2 { get; set; }
    }

    public interface ITestInheritedBase3
    {
        int BaseField3 { get; set; }
    }

    public interface ITestPlainType : ITestInheritedBase2, ITestInheritedBase3
    {
        int IntProperty { get; set; }
        decimal DecimalProperty { get; set; }
        string StringProperty { get; set; }
        object GetterOnly { get; }
        object SetterOnly { set; }
        
        long this [long index] { get; set; }
        int this [int index] { get; }
        Poco this [Poco index] { set; }
        
        event TestHandler Event1;
        event TestHandlerValue Event2;
        event TestHandlerReference Event3;
        void RaiseEvent1();
        int RaiseEvent2(int data);
        Poco RaiseEvent3(Poco data);
        
        Poco PlainMethod(int number, Poco data);
        int ParamsMethod(int data1, params object[] dataParams);
        int OptionalMethod(int data1, int data2 = -1); 
        int ImplementedMethod(int data1, int data2)
        {
            return data1 + data2;
        }
        
        void ThrowMethod(Exception e);
        
    }
}