namespace Lide.AsyncProxy.Tests.Stubs
{
    public delegate TType TesterGenericHandler<TType>(TType input)
        where TType : class;
    
    public interface ITesterGenericType<TType>
        where TType : class
    {
        TType ValueProperty { get; set; }
        TType this [TType index] { get; set; }
        event TesterGenericHandler<TType> Event;
        TType RaiseEvent(TType data);
        TType Method(TType data);
    }
}