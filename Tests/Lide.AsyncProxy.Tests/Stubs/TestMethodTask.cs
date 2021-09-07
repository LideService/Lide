using System.Threading.Tasks;

namespace Lide.AsyncProxy.Tests.Stubs
{
    public interface ITestMethodTask
    {
        Task Method1();
        Task<int> Method2();
        Task AsyncMethod1();
        Task<int> AsyncMethod2();
    }

    public class TestMethodTask : ITestMethodTask
    {
        public Task Method1()
        {
            return Task.CompletedTask;
        }

        public Task<int> Method2()
        {
            return Task.FromResult(7);
        }

        public async Task AsyncMethod1()
        {
            await Task.Delay(1);
        }

        public async Task<int> AsyncMethod2()
        {
            await Task.Delay(1);
            return 13;
        }
    }
}