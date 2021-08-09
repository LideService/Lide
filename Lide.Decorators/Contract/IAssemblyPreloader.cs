using System.Collections.Generic;
using System.Reflection;

namespace Lide.Decorators.Contract
{
    public interface IAssemblyPreloader
    {
        List<Assembly> GetAssemblies();
    }
}