namespace Lide.Core.Contract.Wrapper
{
    public interface ILoggerWrapper
    {
        void Log(string message);
        void LogError(string message);
    }
}