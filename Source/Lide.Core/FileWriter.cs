using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;

namespace Lide.Core
{
    public class FileWriter : IFileWriter
    {
        private readonly IFileNameProvider _fileNameProvider;
        private readonly Task _writer;
        private readonly Dictionary<string, FileStream> _fileHandles;

        private ConcurrentQueue<QueueData> _queue;
        private bool _keepAlive;

        public FileWriter(IFileNameProvider fileNameProvider)
        {
            _fileNameProvider = fileNameProvider;
            _fileHandles = new Dictionary<string, FileStream>();
            _queue = new ConcurrentQueue<QueueData>();
            _writer = Task.Run(Writer);
            _keepAlive = true;
        }

        public void AddToQueue(Func<byte[]> serializer, string decoratorId)
        {
            if (_keepAlive)
            {
                _queue.Enqueue(new QueueData(serializer, decoratorId));
            }
        }

        public string GetFileName(string decoratorId)
        {
            return _fileHandles.ContainsKey(decoratorId)
                ? _fileHandles[decoratorId].Name
                : string.Empty;
        }

        public async Task KillQueue()
        {
            _keepAlive = false;
            await _writer.ConfigureAwait(false);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources().Wait();
            GC.SuppressFinalize(this);
        }

        private async Task Writer()
        {
            while (_keepAlive)
            {
                while (_queue.TryDequeue(out var queueData))
                {
                    if (!_fileHandles.ContainsKey(queueData.DecoratorId))
                    {
                        var filePath = _fileNameProvider.GetTempFileName(queueData.DecoratorId);
                        var fileHandle = File.OpenWrite(filePath);
                        _fileHandles[queueData.DecoratorId] = fileHandle;
                    }

                    var data = queueData.Serializer();
                    await _fileHandles[queueData.DecoratorId].WriteAsync(data).ConfigureAwait(false);
                    await _fileHandles[queueData.DecoratorId].FlushAsync().ConfigureAwait(false);
                }

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        private async Task ReleaseUnmanagedResources()
        {
            var stop = KillQueue();
            _queue = new ConcurrentQueue<QueueData>();
            await stop.ConfigureAwait(false);

            var tasks = _fileHandles.Values.Select(async fileHandle =>
            {
                await fileHandle.FlushAsync().ConfigureAwait(false);
                await fileHandle.DisposeAsync().ConfigureAwait(false);
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private class QueueData
        {
            public QueueData(Func<byte[]> serializer, string decoratorId)
            {
                Serializer = serializer;
                DecoratorId = decoratorId;
            }

            public Func<byte[]> Serializer { get; }
            public string DecoratorId { get; }
        }
    }
}