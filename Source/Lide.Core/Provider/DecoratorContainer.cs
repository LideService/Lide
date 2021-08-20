using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.TracingProxy.Contract;

namespace Lide.Core.Provider
{
    [SuppressMessage("Microsoft", "CA1031", Justification = "Regardless of exception must not throw")]
    public class DecoratorContainer : IDecoratorContainer
    {
        private readonly List<IObjectDecorator> _decorators = new ();

        public DecoratorContainer(
            IAssemblyPreloader assemblyPreloader,
            IServiceProvider serviceProvider,
            IActivatorFacade activatorFacade,
            ILoggerFacade logger)
        {
            var types = assemblyPreloader.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IObjectDecorator).IsAssignableFrom(p))
                .Where(p => !p.IsInterface)
                .ToList();

            types
                .ForEach(decoratorType =>
                {
                    try
                    {
                        var decoratorObject = activatorFacade.CreateInstance(serviceProvider, decoratorType);
                        var decorator = (IObjectDecorator)decoratorObject;
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

        public IObjectDecorator[] GetDecorators(ISettingsProvider settingsProvider)
        {
            return _decorators
                .Where(x => settingsProvider.AppliedDecorators.Contains(x.Id))
                .Where(x => !x.IsVolatile || settingsProvider.AllowVolatileDecorators)
                .ToArray();
        }
    }
}