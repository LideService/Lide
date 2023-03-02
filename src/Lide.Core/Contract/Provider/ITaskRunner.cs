using System;
using System.Threading.Tasks;

namespace Lide.Core.Contract.Provider;

public interface ITaskRunner
{
    void AddToQueue(Func<Task> task);
    Task KillQueue();
    Task WaitQueue();
}