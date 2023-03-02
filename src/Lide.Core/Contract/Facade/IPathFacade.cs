namespace Lide.Core.Contract.Facade;

public interface IPathFacade
{
    string GetTempPath();
    string Combine(params string[] paths);
}