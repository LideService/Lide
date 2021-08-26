using System.Collections.Generic;

namespace Lide.Core.Model.Settings
{
    public class TypeGroups
    {
        public TypeGroups()
        {
            Types = new List<string>();
            Namespaces = new List<string>();
            Assemblies = new List<string>();
        }

        public List<string> Types { get; set; }
        public List<string> Namespaces { get; set; }
        public List<string> Assemblies { get; set; }
    }
}