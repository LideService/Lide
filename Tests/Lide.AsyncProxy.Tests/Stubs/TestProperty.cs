namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestProperty
    {
        public int IntProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public string StringProperty { get; set; }
        public object GetterOnly { get; }
        public object SetterOnly { set; }
    }
    
    public class TestProperty : ITestProperty
    {
        private object _value;
        
        public int IntProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public string StringProperty { get; set; }
        public object GetterOnly => _value;
        public object SetterOnly
        {
            set => _value = value;
        }
    }
}