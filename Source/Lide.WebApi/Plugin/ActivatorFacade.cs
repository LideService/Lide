using System;
using Lide.Core.Contract.Facade;
using Microsoft.Extensions.DependencyInjection;

namespace Lide.WebApi.Plugin
{
    public class ActivatorFacade : IActivatorFacade
    {
        public object CreateInstance(IServiceProvider serviceProvider, Type instanceType)
        {
            return ActivatorUtilities.CreateInstance(serviceProvider, instanceType);
        }
    }
}