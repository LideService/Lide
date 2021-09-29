namespace Lide.Core.Contract.Provider
{
    public interface IPathProvider
    {
        string GetDecoratorFilePath(string decoratorId, bool includeTime);
    }
}