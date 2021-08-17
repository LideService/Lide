namespace Lide.Core.Contract.Provider
{
    public interface ISettingsProvider
    {
        bool SearchHttpBodyOrQuery { get;  }
        bool OverrideDecorators { get;  }
        bool AllowVolatileDecorators { get; }
        string[] ExcludedTypes { get; }
        string[] ExcludedNamespaces { get; }
        string[] ExcludedAssemblies { get; }
        string[] AppliedDecorators { get; }
    }
}