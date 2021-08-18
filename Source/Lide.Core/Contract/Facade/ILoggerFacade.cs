namespace Lide.Core.Contract.Facade
{
    public interface ILoggerFacade
    {
        void Log(string message);
        void LogError(string message);
    }
}