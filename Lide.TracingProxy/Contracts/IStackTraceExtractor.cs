using Lide.TracingProxy.Model;

namespace Lide.TracingProxy.Contracts
{
    public interface IStackTraceExtractor
    {
        CallerInformation ExtractCallerInformation();
    }
}