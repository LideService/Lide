using System.Linq;
using Lide.Decorators.Contract;

namespace Lide.Decorators.Model
{
    public class LideAppSettings 
    {
        public bool SearchHttpBodyOrQuery { get; set; }
        public string VolatileKey { get; set; }
        public string[] ExcludedTypes { get; set; }
        public string[] ExcludedNamespaces { get; set; }
        public string[] ExcludedAssemblies { get; set; }
        public string[] AppliedDecorators { get; set; }
    }
}