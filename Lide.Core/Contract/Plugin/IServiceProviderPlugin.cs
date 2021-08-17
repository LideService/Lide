using System;

namespace Lide.Core.Contract.Plugin
{
    public interface IServiceProviderPlugin
    {
        Type Type { get; set; }
        object GetService(object originalObject);
    }
}