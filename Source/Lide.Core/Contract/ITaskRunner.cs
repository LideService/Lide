using System.Threading.Tasks;

namespace Lide.Core.Contract
{
    public interface ITaskRunner
    {
        void AddToQueue(Task task);
        Task KillQueue();
    }
}