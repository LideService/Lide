using System.Collections.Generic;
using Lide.Decorators.ObjectDecorator;
using Lide.TracingProxy.Contract;

namespace Lide.Decorators.DataProcessors
{
    public static class DecoratorContainer
    {
        private static readonly List<IObjectDecorator> Decorators = new ();
        
        static DecoratorContainer()
        {
            Decorators.Add(new ConsoleDecorator());
        }
        
        public static void AddDecorator()
        {
            
        }
    }
}