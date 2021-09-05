using System.Threading.Tasks;

namespace Lide.Core.Contract.Provider
{
    public interface ITaskRunner
    {
        void AddToQueue(Task task);
        Task KillQueue();
    }
}