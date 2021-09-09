using System;
using System.Runtime.ExceptionServices;

namespace Lide.AsyncProxy.Tests.Stubs
{
    public class TesterPlainType : ITesterPlainType
    {
        private object _getterSetterValue;
        private long _longIndexer;
        private int _intIndexer;

        #region inheritance
        public int BaseField1 { get; set; }
        public int BaseField2 { get; set; }
        public int BaseField3 { get; set; }
        #endregion
        
        #region properties
        public int IntProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public string StringProperty { get; set; }
        public object GetterOnly => _getterSetterValue;
        public object SetterOnly
        {
            set => _getterSetterValue = value;
        }
        #endregion
        
        #region indexers
        public long this[long index]
        {
            get => _longIndexer;
            set => _longIndexer = value;
        }

        public int this[int index] => _intIndexer + index;

        public Poco this[Poco index]
        {
            set => _intIndexer = index.IntField + value.IntField;
        }


        #endregion
        
        #region methods
        public Poco PlainMethod(int number, Poco data)
        {
            data.IntField = data.IntField + number;
            data.StringField = $"{data.StringField}.{number}";
            return data;
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
        #endregion

        #region events
        public event TesterHandler Event1;
        public event TesterHandlerValue Event2;
        public event TesterHandlerReference Event3;
        public void RaiseEvent1()
        {
            Event1?.Invoke();
        }
        
        public int RaiseEvent2(int data)
        {
            return Event2?.Invoke(data) ?? -1;
        }

        public Poco RaiseEvent3(Poco data)
        {
            return Event3?.Invoke(data);
        }
        #endregion
    }
}