using System.Threading.Tasks;

namespace Lide.AsyncProxy.Tests.Stubs
{
    public class TestAsyncType : ITestAsyncType
    {
        public Task Method1()
        {
            return Task.CompletedTask;
        }

        public Task<Poco> Method2(Poco data)
        {
            return Task.FromResult(data);
        }

        public async Task AsyncMethod1()
        {
            await Task.Delay(1);
        }

        public async Task<Poco> AsyncMethod2(Poco data)
        {
            await Task.Delay(1);
            return data;
        }
    }
}