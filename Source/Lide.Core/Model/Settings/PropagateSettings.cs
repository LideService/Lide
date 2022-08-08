using System.Collections.Generic;

namespace Lide.Core.Model.Settings
{
    public class PropagateSettings
    {
        public bool OverrideInclusionPattern { get; set; }
        public bool OverrideDecoratorsWithPattern { get; set; }
        public bool OverrideAddressesPattern { get; set; }

        public string VolatileKey { get; set; } = string.Empty;
        public string EnabledKey { get; set; } = string.Empty;
        public string TypesInclusionPattern { get; set; } = string.Empty;
        public string AddressesInclusionPattern { get; set; } = string.Empty;
        public List<string> DecoratorsWithPattern { get; set; } = new ();
    }
}