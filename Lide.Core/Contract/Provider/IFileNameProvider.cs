namespace Lide.Core.Contract.Provider
{
    public interface IFileNameProvider
    {
        string GetTempFileName(string decoratorId);
    }
}