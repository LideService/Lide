using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Core.Contract.Provider;
using Lide.Core.Contract.Wrapper;
using Lide.TracingProxy.Contract;

namespace Lide.Core.Provider
{
    public class DecoratorProvider : IDecoratorProvider
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly List<IObjectDecorator> _decorators = new();

        public DecoratorProvider(
            ISettingsProvider settingsProvider, 
            IAssemblyPreloader assemblyPreloader, 
            IServiceProvider serviceProvider, 
            IActivatorWrapper activatorWrapper,
            ILoggerWrapper logger)
        {
            _settingsProvider = settingsProvider;
            assemblyPreloader.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IObjectDecorator).IsAssignableFrom(p))
                .ToList()
                .ForEach(decoratorType =>
                {
                    
                    try
                    {
                        var decoratorObject = activatorWrapper.CreateInstance(serviceProvider, decoratorType);
                        var decorator = (IObjectDecorator) decoratorObject;
                        ConfigureDecorator(decorator);
                        logger.Log($"Decorator configured: {decoratorType.Name}; Id: {decorator.Id};");
                    }
                    catch
                    {
                        logger.LogError($"Couldn't automatically create decorator of type {decoratorType.Name}");
                    }
                });
        }

        public void ConfigureDecorator(IObjectDecorator decorator)
        {
            if (decorator != null && _decorators.All(x => x.Id != decorator.Id))
            {
                _decorators.Add(decorator);
            }
        }

        public IObjectDecorator[] GetDecorators()
        {
            return _decorators
                .Where(x => _settingsProvider.AppliedDecorators.Contains(x.Id))
                .Where(x => !x.IsVolatile || _settingsProvider.AllowVolatileDecorators)
                .ToArray();
        }
    }
}