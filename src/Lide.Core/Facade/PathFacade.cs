using System.IO;
using Lide.Core.Contract.Facade;

namespace Lide.Core.Facade;

public class PathFacade : IPathFacade
{
    public string GetTempPath()
    {
        return Path.GetTempPath();
    }

    public string Combine(params string[] paths)
    {
        return Path.Combine(paths);
    }
}