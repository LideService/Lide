using System;

namespace Lide.Core.Contract.Plugin
{
    public interface IServiceProviderPlugin
    {
        Type Type { get; }
        object GetService(object originalObject);
    }
}