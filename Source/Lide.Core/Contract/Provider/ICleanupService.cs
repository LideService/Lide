using System;

namespace Lide.Core.Contract.Provider
{
    public interface ICleanupService : IAsyncDisposable
    {
        public void RegisterCleanupFile(string filePath);
        public void CleanUp();
    }
}