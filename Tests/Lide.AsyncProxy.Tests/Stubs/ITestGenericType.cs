namespace Lide.AsyncProxy.Tests.Stubs
{
    public delegate TType TestGenericHandler<TType>(TType input)
        where TType : class;
    
    public interface ITestGenericType<TType>
        where TType : class
    {
        TType ValueProperty { get; set; }
        TType this [TType index] { get; set; }
        event TestGenericHandler<TType> Event;
        TType RaiseEvent(TType data);
        TType Method(TType data);
    }
}