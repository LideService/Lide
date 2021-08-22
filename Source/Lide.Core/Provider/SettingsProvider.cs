using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Lide.Core.Contract.Provider;
using Lide.Core.Model;

namespace Lide.Core.Provider
{
    [SuppressMessage("Microsoft", "CA1819", Justification = "Easier with array than list")]
    public class SettingsProvider : ISettingsProvider
    {
        private static readonly string[] LideAssemblies = new[]
        {
            "Lide.",
            "Microsoft.",
        };

        public LideAppSettings LideAppSettings { get; set; }
        public LidePropagateSettings LidePropagateSettings { get; set; }

        public bool SearchHttpBodyOrQuery => LideAppSettings.SearchHttpBody;
        public bool AllowVolatileDecorators => LideAppSettings.VolatileKey == LidePropagateSettings.VolatileKey;
        public string[] ExcludedTypes => LideAppSettings.ExcludedTypes.Concat(LidePropagateSettings.ExcludedTypes).ToArray();
        public string[] ExcludedNamespaces => LideAppSettings.ExcludedNamespaces.Concat(LidePropagateSettings.ExcludedNamespaces).ToArray();
        public string[] ExcludedAssemblies => LideAssemblies
            .Concat(LideAppSettings.ExcludedAssemblies)
            .Concat(LidePropagateSettings.ExcludedAssemblies).ToArray();
        public string[] AppliedDecorators =>
            LidePropagateSettings.OverrideDecorators
                ? LidePropagateSettings.AppliedDecorators
                : LideAppSettings.AppliedDecorators.Concat(LidePropagateSettings.AppliedDecorators).ToArray();
    }
}