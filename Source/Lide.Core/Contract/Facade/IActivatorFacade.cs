using System;

namespace Lide.Core.Contract.Facade
{
    public interface IActivatorFacade
    {
        object CreateInstance(IServiceProvider serviceProvider, Type instanceType);
    }
}