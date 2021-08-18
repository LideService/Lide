using System;

namespace Lide.Core.Contract.Facade
{
    public interface IActivatorWrapper
    {
        object CreateInstance(IServiceProvider serviceProvider, Type instanceType);
    }
}