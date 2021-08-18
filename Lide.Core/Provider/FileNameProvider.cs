using System;
using System.IO;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class FileNameProvider : IFileNameProvider
    {
        public string GetTempFileName(string decoratorId)
        {
            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            var path = Path.GetTempPath();
            var fileName = $"Lide.{date}.{decoratorId}";
            return Path.Combine(path, fileName);
        }
    }
}