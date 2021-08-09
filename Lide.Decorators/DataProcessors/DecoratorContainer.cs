using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Decorators.Contract;
using Lide.Decorators.Model;
using Lide.TracingProxy.Contract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lide.Decorators.DataProcessors
{
    public class DecoratorContainer : IDecoratorContainer
    {
        private readonly ILideSettingsProcessor _lideSettingsProcessor;
        private readonly List<IObjectDecorator> _decorators = new();

        public DecoratorContainer(
            ILideSettingsProcessor lideSettingsProcessor, 
            IAssemblyPreloader assemblyPreloader, 
            IServiceProvider serviceProvider,
            ILogger<DecoratorContainer> logger)
        {
            _lideSettingsProcessor = lideSettingsProcessor;
            assemblyPreloader.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IObjectDecorator).IsAssignableFrom(p))
                .ToList()
                .ForEach(decoratorType =>
                {
                    SafeExecute(() =>
                    {
                        var decorator = ActivatorUtilities.CreateInstance(serviceProvider, decoratorType);
                        ConfigureDecorator((IObjectDecorator) decorator);
                    });
                });
            
            logger.LogTrace("Available object decorators:");
            _decorators.ForEach(x => logger.LogTrace($"Type: {x.GetType().Name}; Id: {x.Id};"));
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
                .Where(x => _lideSettingsProcessor.AppliedDecorators.Contains(x.Id))
                .Where(x => !x.Volatile || _lideSettingsProcessor.AllowVolatileDecorators)
                .ToArray();
        }

        private static void SafeExecute(Action action)
        {
            
            try
            {
                action?.Invoke();
            }
            catch
            {
                //ignored
            }
        }
    }
}