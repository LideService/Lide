using Lide.TracingProxy.DataProcessors.Model;

namespace Lide.TracingProxy.DataProcessors.Contract
{
    public interface IStackTraceExtractor
    {
        CallerInformation ExtractCallerInformation();
    }
}