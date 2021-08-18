using System.Collections.Generic;
using System.Reflection;

namespace Lide.Core.Contract.Provider
{
    public interface IAssemblyPreloader
    {
        List<Assembly> GetAssemblies();
    }
}