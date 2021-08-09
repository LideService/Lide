using System.Linq;
using Lide.Decorators.Contract;
using Lide.Decorators.Model;

namespace Lide.Decorators.DataProcessors
{
    public class LideSettingsProcessor : ILideSettingsProcessor
    {
        public static LideAppSettings LideAppSettings { get; set; }
        public static LidePropagateSettings LidePropagateSettings { get; set; }

        public bool SearchHttpBodyOrQuery => LideAppSettings.SearchHttpBodyOrQuery;
        public bool OverrideDecorators => LidePropagateSettings.OverrideDecorators;
        public bool AllowVolatileDecorators => LideAppSettings.VolatileKey == LidePropagateSettings.VolatileKey;
        public string[] ExcludedTypes => LideAppSettings.ExcludedTypes.Concat(LidePropagateSettings.ExcludedTypes).ToArray();
        public string[] ExcludedNamespaces => LideAppSettings.ExcludedNamespaces.Concat(LidePropagateSettings.ExcludedNamespaces).ToArray();
        public string[] ExcludedAssemblies => LideAppSettings.ExcludedAssemblies.Concat(LidePropagateSettings.ExcludedAssemblies).ToArray();
        public string[] AppliedDecorators => LideAppSettings.AppliedDecorators.Concat(LidePropagateSettings.AppliedDecorators).ToArray();
    }
}