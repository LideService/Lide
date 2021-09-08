using System.Threading.Tasks;

namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestAsyncType
    {
        Task Method1();
        Task<Poco> Method2(Poco data);
        Task AsyncMethod1();
        Task<Poco> AsyncMethod2(Poco data);
    }
}