using System.Collections.Generic;

namespace Lide.Core.Model.Settings
{
    public class AppSettings
    {
        public bool DefaultTypeInclusion { get; set; } // whether to include type if not either included or excluded from app/propagate settings
        public bool DefaultAddressInclusion { get; set; } // whether to include address if not either included or excluded from app/propagate settings
        public string VolatileKey { get; set; } = string.Empty;
        public string EnabledKey { get; set; } = string.Empty;
        public string TypesInclusionPattern { get; set; } = string.Empty;
        public string AddressesInclusionPattern { get; set; } = string.Empty;
        public List<string> DecoratorsWithPattern { get; set; } = new ();
    }
}