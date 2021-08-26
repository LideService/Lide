namespace Lide.Core.Model.Settings
{
    public enum InclusionType
    {
        OnlyIncluded,
        AllButExcluded,
        Both, // All but excluded, with all explicitly included, which might've been excluded.
    }
}