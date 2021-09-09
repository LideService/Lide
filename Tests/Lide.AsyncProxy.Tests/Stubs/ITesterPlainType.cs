using System;

namespace Lide.AsyncProxy.Tests.Stubs
{
    public delegate void TesterHandler();
    public delegate int TesterHandlerValue(int data);
    public delegate Poco TesterHandlerReference(Poco data);
    
    public interface ITesterInheritedBase1
    {
        int BaseField1 { get; set; }
    }

    public interface ITesterInheritedBase2 : ITesterInheritedBase1
    {
        int BaseField2 { get; set; }
    }

    public interface ITesterInheritedBase3
    {
        int BaseField3 { get; set; }
    }

    public interface ITesterPlainType : ITesterInheritedBase2, ITesterInheritedBase3
    {
        int IntProperty { get; set; }
        decimal DecimalProperty { get; set; }
        string StringProperty { get; set; }
        object GetterOnly { get; }
        object SetterOnly { set; }
        
        long this [long index] { get; set; }
        int this [int index] { get; }
        Poco this [Poco index] { set; }
        
        event TesterHandler Event1;
        event TesterHandlerValue Event2;
        event TesterHandlerReference Event3;
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