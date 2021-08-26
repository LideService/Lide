using System.Collections.Generic;

namespace Lide.Core.Model.Settings
{
    public class PropagateSettings
    {
        public PropagateSettings()
        {
            Decorators = new List<string>();
            GroupsInclusion = new GroupsInclusion();
        }

        public bool OverrideDecorators { get; set; }
        public bool OverrideInclusionType { get; set; }
        public bool OverrideGroupInclusions { get; set; }
        public string VolatileKey { get; set; }
        public string EnabledKey { get; set; }
        public GroupsInclusion GroupsInclusion { get; set; }
        public List<string> Decorators { get; set; }
    }
}