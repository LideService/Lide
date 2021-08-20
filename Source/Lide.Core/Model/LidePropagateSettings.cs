using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Lide.Core.Model
{
    [SuppressMessage("Microsoft", "CA1819", Justification = "Easier with array than list")]
    public class LidePropagateSettings
    {
        private const int AppliedDecoratorsIndex = 0;
        private const int ExcludedAssembliesIndex = 1;
        private const int ExcludedNamespacesIndex = 2;
        private const int ExcludedTypesIndex = 3;
        private const int OverrideDecoratorsIndex = 4;
        private const string FieldSeparator = ";";
        private const string ArraySeparator = ",";

        public LidePropagateSettings()
        {
        }

        public LidePropagateSettings(string serialized)
        {
            if (string.IsNullOrEmpty(serialized))
            {
                return;
            }

            var parsed = serialized.Split(FieldSeparator);
            AppliedDecorators = parsed[AppliedDecoratorsIndex].Split(',');
            ExcludedAssemblies = parsed[ExcludedAssembliesIndex].Split(',');
            ExcludedNamespaces = parsed[ExcludedNamespacesIndex].Split(',');
            ExcludedTypes = parsed[ExcludedTypesIndex].Split(',');
            OverrideDecorators = parsed[OverrideDecoratorsIndex] == "Y" ? true : false;
        }

        public bool OverrideDecorators { get; set; }
        public string VolatileKey { get; set; }
        public string[] ExcludedTypes { get; set; }
        public string[] ExcludedNamespaces { get; set; }
        public string[] ExcludedAssemblies { get; set; }
        public string[] AppliedDecorators { get; set; }

        public override string ToString()
        {
            return ConstructSerialized(
                (string.Join(ArraySeparator, AppliedDecorators), AppliedDecoratorsIndex),
                (string.Join(ArraySeparator, ExcludedAssemblies), ExcludedAssembliesIndex),
                (string.Join(ArraySeparator, ExcludedNamespaces), ExcludedNamespacesIndex),
                (string.Join(ArraySeparator, ExcludedTypes), ExcludedTypesIndex),
                (OverrideDecorators ? "Y" : "N", OverrideDecoratorsIndex));
        }

        private static string ConstructSerialized(params (string field, int index)[] fields)
        {
            return string.Join(FieldSeparator, fields.OrderBy(x => x.index));
        }
    }
}