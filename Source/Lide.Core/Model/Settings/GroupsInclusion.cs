namespace Lide.Core.Model.Settings
{
    public class GroupsInclusion
    {
        public GroupsInclusion()
        {
            InclusionType = InclusionType.OnlyIncluded;
            Included = new TypeGroups();
            Excluded = new TypeGroups();
        }

        public InclusionType InclusionType { get; set; }
        public TypeGroups Included { get; set; }
        public TypeGroups Excluded { get; set; }
    }
}