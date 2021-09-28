using System.Collections.Generic;

namespace Lide.Core.Model.Settings
{
    public class AppSettings
    {
        public bool RecordRequestBody { get; set; }
        public bool SearchHttpBody { get; set; }
        public string VolatileKey { get; set; } = string.Empty;
        public string EnabledKey { get; set; } = string.Empty;
        public string InclusionPattern { get; set; } = string.Empty;
        public List<string> DecoratorsWithPattern { get; set; } = new ();
    }
}