using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lide.Decorators.Contract;

namespace Lide.Decorators.DataProcessors
{
    public class AssemblyPreloader : IAssemblyPreloader
    {
        public List<Assembly> GetAssemblies()
        {
            var returnAssemblies = new List<Assembly>();
            var loadedAssemblies = new HashSet<string>();
            var assembliesToCheck = new Queue<Assembly>();

            assembliesToCheck.Enqueue(Assembly.GetEntryAssembly());

            while(assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();

                foreach(var reference in assemblyToCheck.GetReferencedAssemblies())
                {
                    if(!loadedAssemblies.Contains(reference.FullName))
                    {
                        var assembly = Assembly.Load(reference);
                        assembliesToCheck.Enqueue(assembly);
                        loadedAssemblies.Add(reference.FullName);
                        returnAssemblies.Add(assembly);
                    }
                }
            }

            return returnAssemblies;
        }
    }
}