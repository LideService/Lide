using Lide.TracingProxy.Contract;

namespace Lide.Core.Contract.Provider
{
    public interface IDecoratorContainer
    {
        void ConfigureDecorator(IObjectDecorator decorator);
        IObjectDecorator[] GetDecorators(ISettingsProvider settingsProvider);
    }
}