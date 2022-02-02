using System.Collections.Generic;

namespace Lide.Core.Model.Settings
{
    public class AppSettings
    {
        public bool DefaultTypeInclusion { get; set; }
        public bool DefaultAddressInclusion { get; set; }
        public bool SearchHttpBody { get; set; }
        public string VolatileKey { get; set; } = string.Empty;
        public string EnabledKey { get; set; } = string.Empty;
        public string TypesInclusionPattern { get; set; } = string.Empty;
        public string AddressesInclusionPattern { get; set; } = string.Empty;
        public List<string> DecoratorsWithPattern { get; set; } = new ();
    }
}