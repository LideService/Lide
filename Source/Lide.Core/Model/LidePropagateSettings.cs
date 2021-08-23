using System;
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
        private const int IncludedTypesIndex = 4;
        private const int IncludedNamespacesIndex = 5;
        private const int IncludedAssembliesIndex = 6;
        private const int OverrideDecoratorsIndex = 7;
        private const int OverrideInclusionTypeIndex = 8;
        private const int InclusionTypeIndex = 9;
        private const string FieldSeparator = ";";
        private const string ArraySeparator = ",";

        public LidePropagateSettings()
        {
            ExcludedTypes = Array.Empty<string>();
            ExcludedNamespaces = Array.Empty<string>();
            ExcludedAssemblies = Array.Empty<string>();
            IncludedTypes = Array.Empty<string>();
            IncludedNamespaces = Array.Empty<string>();
            IncludedAssemblies = Array.Empty<string>();
            AppliedDecorators = Array.Empty<string>();
        }

        public LidePropagateSettings(string serialized)
            : this()
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
            IncludedAssemblies = parsed[IncludedAssembliesIndex].Split(',');
            IncludedNamespaces = parsed[IncludedNamespacesIndex].Split(',');
            IncludedTypes = parsed[IncludedTypesIndex].Split(',');
            OverrideDecorators = parsed[OverrideDecoratorsIndex] == "Y" ? true : false;
            OverrideInclusionType = parsed[OverrideInclusionTypeIndex] == "Y" ? true : false;
            InclusionType = (InclusionType)Convert.ToInt32(parsed[OverrideInclusionTypeIndex]);
        }

        public bool OverrideDecorators { get; set; }
        public bool OverrideInclusionType { get; set; }
        public InclusionType InclusionType { get; set; }
        public string VolatileKey { get; set; }
        public string[] ExcludedTypes { get; set; }
        public string[] ExcludedNamespaces { get; set; }
        public string[] ExcludedAssemblies { get; set; }
        public string[] IncludedTypes { get; set; }
        public string[] IncludedNamespaces { get; set; }
        public string[] IncludedAssemblies { get; set; }
        public string[] AppliedDecorators { get; set; }

        public override string ToString()
        {
            return ConstructSerialized(
                (string.Join(ArraySeparator, AppliedDecorators), AppliedDecoratorsIndex),
                (string.Join(ArraySeparator, ExcludedAssemblies), ExcludedAssembliesIndex),
                (string.Join(ArraySeparator, ExcludedNamespaces), ExcludedNamespacesIndex),
                (string.Join(ArraySeparator, ExcludedTypes), ExcludedTypesIndex),
                (string.Join(ArraySeparator, IncludedAssemblies), IncludedAssembliesIndex),
                (string.Join(ArraySeparator, IncludedNamespaces), IncludedNamespacesIndex),
                (string.Join(ArraySeparator, IncludedTypes), IncludedTypesIndex),
                (OverrideDecorators ? "Y" : "N", OverrideDecoratorsIndex),
                (OverrideInclusionType ? "Y" : "N", OverrideInclusionTypeIndex),
                (InclusionType.ToString(), InclusionTypeIndex));
        }

        private static string ConstructSerialized(params (string field, int index)[] fields)
        {
            return string.Join(FieldSeparator, fields.OrderBy(x => x.index));
        }
    }
}