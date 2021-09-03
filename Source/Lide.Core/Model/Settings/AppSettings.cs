using System.Collections.Generic;

namespace Lide.Core.Model.Settings
{
    public class AppSettings
    {
        public AppSettings()
        {
            Decorators = new List<string>();
            GroupsInclusion = new GroupsInclusion();
        }

        public bool RecordRequestBody { get; set; }
        public bool SearchHttpBody { get; set; }
        public bool DisableOnlyExecutingAssemblyAlike { get; set; }
        public string VolatileKey { get; set; }
        public string EnabledKey { get; set; }
        public GroupsInclusion GroupsInclusion { get; set; }
        public List<string> Decorators { get; set; }
    }
}