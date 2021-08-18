using System;
using System.Threading.Tasks;

namespace Lide.Core.Contract.Facade
{
    public interface IFileWriter : IDisposable
    {
        void AddToQueue(Func<byte[]> serializer, string decoratorId);
        string GetFileName(string decoratorId);
        Task Stop();
    }
}