namespace Lide.Decorators.DataProcessors
{
    public static class Settings
    {
        public static bool AlwaysEnabled { get; set; }
        public static string AlwaysEnabledKey { get; set; }
        public static string[] ExcludedTypes { get; set; }
        public static string[] ExcludedNamespaces { get; set; }
        public static string[] ExcludedAssemblies { get; set; }
    }
}