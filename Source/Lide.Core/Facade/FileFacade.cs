using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lide.Core.Contract.Facade;

namespace Lide.Core.Facade
{
    [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "using await .ConfigureAwait nonsense")]
    public class FileFacade : IFileFacade
    {
        private readonly IDateTimeFacade _dateTimeFacade;
        private static int _fileCount;

        public FileFacade(IDateTimeFacade dateTimeFacade)
        {
            _dateTimeFacade = dateTimeFacade;
        }

        public string GetFileName(string id = null)
        {
            var nextId = Interlocked.Increment(ref _fileCount);
            var sb = new StringBuilder();
            sb.Append("Lide");

            if (!string.IsNullOrEmpty(id))
            {
                sb.Append('.');
                sb.Append(id);
            }

            sb.Append('.');
            sb.Append(nextId);
            sb.Append('.');
            sb.Append(_dateTimeFacade.GetDateNow().ToString("yyyyMMdd_hhmmss"));
            return sb.ToString();
        }

        public Stream OpenFile(string filePath)
        {
            return new FileStream(filePath, FileMode.Append, FileAccess.ReadWrite, FileShare.None);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public Task<byte[]> ReadWholeFle(string filePath)
        {
            return File.ReadAllBytesAsync(filePath);
        }

        public Task WriteWholeFile(string filePath, byte[] data)
        {
            return File.WriteAllBytesAsync(filePath, data);
        }
    }
}