using Lide.TracingProxy.Contract;

namespace Lide.Core.Contract.Provider
{
    public interface IDecoratorProvider
    {
        void ConfigureDecorator(IObjectDecorator decorator);
        IObjectDecorator[] GetDecorators();
    }
}