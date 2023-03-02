using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lide.Core.Contract;

namespace Lide.Core.Provider;

public class DeterministicTypeProvider
{
    private readonly IMethodDependenciesProvider _methodDependenciesProvider;

    private readonly HashSet<Type> _volatileTypes = new()
    {
        typeof(DateTime),
        typeof(Random),
        typeof(Environment),
        typeof(TimeZoneInfo),
        typeof(DateTimeOffset),
        typeof(System.IO.Directory),
        typeof(System.IO.File),
        typeof(System.IO.Path),
        typeof(System.IO.DirectoryInfo),
        typeof(System.IO.DriveInfo),
        typeof(System.IO.FileInfo),
        typeof(System.IO.FileStream),
        typeof(System.IO.FileSystemInfo),
        typeof(System.IO.FileSystemWatcher),
    };

    private readonly HashSet<string> _volatileFullNamespaces = new()
    {
        "System.Net",
    };

    private readonly HashSet<string> _volatilePartialNamespaces = new()
    {
        "System.Net",
    };

    public DeterministicTypeProvider(IMethodDependenciesProvider methodDependenciesProvider)
    {
        _methodDependenciesProvider = methodDependenciesProvider;
    }

    public bool IsMethodDeterministic(MethodInfo methodInfo)
    {
        var dependencies = _methodDependenciesProvider.GetDependencies(methodInfo);
        if (dependencies.Any(x => _volatileTypes.Contains(x)))
        {
            return false;
        }

        var namespaces = dependencies.Select(x => x.Namespace).Where(x => !string.IsNullOrEmpty(x)).ToList();
        if (namespaces.Any(x => _volatileFullNamespaces.Contains(x)))
        {
            return false;
        }

        if (namespaces.Any(x => _volatilePartialNamespaces.Any(x.StartsWith)))
        {
            return false;
        }

        return true;
    }
}