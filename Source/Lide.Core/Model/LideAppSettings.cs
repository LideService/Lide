using System;
using System.Diagnostics.CodeAnalysis;

namespace Lide.Core.Model
{
    [SuppressMessage("Microsoft", "CA1819", Justification = "Easier with array than list")]
    public class LideAppSettings
    {
        public LideAppSettings()
        {
            ExcludedTypes = Array.Empty<string>();
            ExcludedNamespaces = Array.Empty<string>();
            ExcludedAssemblies = Array.Empty<string>();
            AppliedDecorators = Array.Empty<string>();
        }

        public bool SearchHttpBody { get; set; }
        public string VolatileKey { get; set; }
        public string[] ExcludedTypes { get; set; }
        public string[] ExcludedNamespaces { get; set; }
        public string[] ExcludedAssemblies { get; set; }
        public string[] AppliedDecorators { get; set; }
    }
}