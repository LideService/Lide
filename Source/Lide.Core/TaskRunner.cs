using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lide.Core.Contract;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;

namespace Lide.Core
{
    public class TaskRunner : ITaskRunner
    {
        private readonly Task _worker;
        private readonly ConcurrentQueue<Task> _queue;
        private bool _keepAlive;

        public TaskRunner(IFileNameProvider fileNameProvider)
        {
            _queue = new ConcurrentQueue<Task>();
            _worker = Task.Run(Writer);
            _keepAlive = true;
        }

        public void AddToQueue(Task task)
        {
            if (_keepAlive)
            {
                _queue.Enqueue(task);
            }
        }

        public async Task KillQueue()
        {
            _keepAlive = false;
            await _worker.ConfigureAwait(false);
        }

        private async Task Writer()
        {
            while (_keepAlive)
            {
                while (_queue.TryDequeue(out var task))
                {
                    task.Start();
                    await task.ConfigureAwait(false);
                }

                await Task.Delay(100).ConfigureAwait(false);
            }
        }
    }
}