using System.IO;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class FileNameProvider : IFileNameProvider
    {
        private readonly IDateTimeFacade _dateTimeFacade;

        public FileNameProvider(IDateTimeFacade dateTimeFacade)
        {
            _dateTimeFacade = dateTimeFacade;
        }

        public string GetTempFileName(string decoratorId)
        {
            var date = _dateTimeFacade.GetDateNow().ToString("yyyyMMddHHmmss");
            var path = Path.GetTempPath();
            var fileName = $"Lide.{date}.{decoratorId}";
            return Path.Combine(path, fileName);
        }
    }
}