using Lide.TracingProxy.Contract;

namespace Lide.Decorators.Contract
{
    public interface IDecoratorContainer
    {
        void ConfigureDecorator(IObjectDecorator decorator);
        IObjectDecorator[] GetDecorators();
    }
}