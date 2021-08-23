using System.Diagnostics.CodeAnalysis;

namespace Lide.Core.Model
{
    [SuppressMessage("Microsoft", "CA1819", Justification = "Easier with array than list")]
    public class DecoratorInclusion
    {
        public string DecoratorId { get; set; }
        public InclusionType InclusionType { get; set; }
        public string[] ExcludedTypes { get; set; }
        public string[] ExcludedNamespaces { get; set; }
        public string[] ExcludedAssemblies { get; set; }
        public string[] IncludedTypes { get; set; }
        public string[] IncludedNamespaces { get; set; }
        public string[] IncludedAssemblies { get; set; }
    }
}