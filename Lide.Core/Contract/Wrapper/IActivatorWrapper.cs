using System;

namespace Lide.Core.Contract.Wrapper
{
    public interface IActivatorWrapper
    {
        object CreateInstance(IServiceProvider serviceProvider, Type instanceType);
    }
}