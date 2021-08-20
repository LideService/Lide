using System.Diagnostics.CodeAnalysis;
using Lide.Core.Model;

namespace Lide.Core.Contract.Provider
{
    [SuppressMessage("Microsoft", "CA1819", Justification = "Easier with array than list")]
    public interface ISettingsProvider
    {
        public LideAppSettings LideAppSettings { get; set; }
        public LidePropagateSettings LidePropagateSettings { get; set; }

        bool SearchHttpBodyOrQuery { get;  }
        bool AllowVolatileDecorators { get; }
        string[] ExcludedTypes { get; }
        string[] ExcludedNamespaces { get; }
        string[] ExcludedAssemblies { get; }
        string[] AppliedDecorators { get; }
    }
}