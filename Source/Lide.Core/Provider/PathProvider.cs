using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;

namespace Lide.Core.Provider
{
    public class PathProvider : IPathProvider
    {
        private readonly IPathFacade _pathFacade;
        private readonly IScopeIdProvider _scopeIdProvider;
        private readonly IDateTimeFacade _dateTimeFacade;

        public PathProvider(
            IPathFacade pathFacade,
            IScopeIdProvider scopeIdProvider,
            IDateTimeFacade dateTimeFacade)
        {
            _pathFacade = pathFacade;
            _scopeIdProvider = scopeIdProvider;
            _dateTimeFacade = dateTimeFacade;
        }

        public string GetDecoratorFilePath(string decoratorId, bool includeTime)
        {
            var tmpPath = _pathFacade.GetTempPath();
            var fileName = $"{_scopeIdProvider.GetRootScopeId()}.{decoratorId}";
            if (includeTime)
            {
                var date = _dateTimeFacade.GetDateNow().ToString("yyyyMMddHHmmss");
                fileName = $"{fileName}.{date}";
            }

            return _pathFacade.Combine(tmpPath, fileName);
        }
    }
}