namespace Lide.Core.Model;

public class SignatureOptions
{
    public bool IncludeAssemblyForBase { get; set; }
    public bool IncludeAssemblyForGeneric { get; set; }
    public bool IncludeAssemblyForParams { get; set; }
    public bool IncludeAssemblyForReturn { get; set; }
    public bool IncludeNamespaceForBase { get; set; }
    public bool IncludeNamespaceForGeneric { get; set; }
    public bool IncludeNamespaceForParams { get; set; }
    public bool IncludeNamespaceForReturn { get; set; }

    public static SignatureOptions AllSet { get; } = new ()
    {
        IncludeAssemblyForBase = true,
        IncludeAssemblyForGeneric = true,
        IncludeAssemblyForParams = true,
        IncludeAssemblyForReturn = true,
        IncludeNamespaceForBase = true,
        IncludeNamespaceForGeneric = true,
        IncludeNamespaceForParams = true,
        IncludeNamespaceForReturn = true,
    };

    public static SignatureOptions OnlyBaseNamespace { get; } = new ()
    {
        IncludeNamespaceForBase = true,
    };
}