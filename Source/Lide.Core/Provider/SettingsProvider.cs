using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model.Settings;

namespace Lide.Core.Provider
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly ISerializerFacade _serializerFacade;
        private readonly TypeGroups _includedFullname;
        private readonly TypeGroups _includedStarts;
        private readonly TypeGroups _includedEnds;
        private readonly TypeGroups _includedStartEnds;

        private readonly TypeGroups _excludedFullname;
        private readonly TypeGroups _excludedStarts;
        private readonly TypeGroups _excludedEnds;
        private readonly TypeGroups _excludedStartEnds;
        private List<string> _decorators;

        private static readonly string[] ExcludeAssemblies = new[]
        {
            "Lide.",
            "Microsoft.",
        };

        public SettingsProvider(ISerializerFacade serializerFacade)
        {
            _serializerFacade = serializerFacade;
            _includedFullname = new TypeGroups();
            _includedStarts = new TypeGroups();
            _includedEnds = new TypeGroups();
            _includedStartEnds = new TypeGroups();

            _excludedFullname = new TypeGroups();
            _excludedStarts = new TypeGroups();
            _excludedEnds = new TypeGroups();
            _excludedStartEnds = new TypeGroups();
        }

        public AppSettings AppSettings { get; private set; }
        public PropagateSettings PropagateSettings { get; private set; }
        public string PropagateSettingsString { get; private set; }

        public bool SearchHttpBodyOrQuery => AppSettings.SearchHttpBody;
        public bool AllowVolatileDecorators => string.IsNullOrEmpty(AppSettings.EnabledKey) || AppSettings.VolatileKey == PropagateSettings.VolatileKey;
        public bool AllowEnablingDecorators => string.IsNullOrEmpty(AppSettings.EnabledKey) || AppSettings.EnabledKey == PropagateSettings.EnabledKey;

        public void SetData(AppSettings appSettings, string propagateSettings)
        {
            AppSettings = appSettings;
            PropagateSettings = _serializerFacade.Deserialize<PropagateSettings>(propagateSettings);
            PropagateSettingsString = propagateSettings;
            BuildIncludedTypes();
            BuildExcludedTypes();
            BuildDecorators();
        }

        public List<string> GetDecorators() => _decorators;

        public bool IsTypeAllowed(Type type, string decoratorId)
        {
            var isIncluded = _includedFullname.Types.Any(x => type.Name == x)
                             || _includedStarts.Types.Any(x => type.Name.StartsWith(x))
                             || _includedEnds.Types.Any(x => type.Name.EndsWith(x))
                             || _includedStartEnds.Types.Any(x => type.Name.StartsWith(x) && type.Name.EndsWith(x));

            var isExcluded = _excludedFullname.Types.Any(x => type.Name == x)
                             || _excludedStarts.Types.Any(x => type.Name.StartsWith(x))
                             || _excludedEnds.Types.Any(x => type.Name.EndsWith(x))
                             || _excludedStartEnds.Types.Any(x => type.Name.StartsWith(x) && type.Name.EndsWith(x));

            return AppSettings.GroupsInclusion.InclusionType switch
            {
                InclusionType.OnlyIncluded => isIncluded,
                InclusionType.AllButExcluded => !isExcluded,
                _ => !isExcluded || isIncluded
            };
        }

        private void BuildDecorators()
        {
            _decorators = PropagateSettings.Decorators;
            if (PropagateSettings.OverrideDecorators)
            {
                _decorators = _decorators.Union(AppSettings.Decorators).ToList();
            }
        }

        private void BuildIncludedTypes()
        {
            FilterOut(_includedFullname, PropagateSettings.GroupsInclusion.Included, IsFullName);
            FilterOut(_includedStarts, PropagateSettings.GroupsInclusion.Included, EndsWith); // ends with after must start with
            FilterOut(_includedEnds, PropagateSettings.GroupsInclusion.Included, StartsWith); // start with after must end with
            FilterOut(_includedStartEnds, PropagateSettings.GroupsInclusion.Included, StartsEndsWith);

            if (PropagateSettings.OverrideGroupInclusions)
            {
                return;
            }

            FilterOut(_includedFullname, AppSettings.GroupsInclusion.Included, IsFullName);
            FilterOut(_includedStarts, AppSettings.GroupsInclusion.Included, EndsWith); // ends with after must start with
            FilterOut(_includedEnds, AppSettings.GroupsInclusion.Included, StartsWith); // start with after must end with
            FilterOut(_includedStartEnds, AppSettings.GroupsInclusion.Included, StartsEndsWith);
        }

        private void BuildExcludedTypes()
        {
            _excludedStarts.Assemblies.AddRange(ExcludeAssemblies);
            FilterOut(_excludedFullname, PropagateSettings.GroupsInclusion.Excluded, IsFullName);
            FilterOut(_excludedStarts, PropagateSettings.GroupsInclusion.Excluded, EndsWith); // ends with after must start with
            FilterOut(_excludedEnds, PropagateSettings.GroupsInclusion.Excluded, StartsWith); // start with after must end with
            FilterOut(_excludedStartEnds, PropagateSettings.GroupsInclusion.Excluded, StartsEndsWith);

            if (PropagateSettings.OverrideGroupInclusions)
            {
                return;
            }

            FilterOut(_excludedFullname, AppSettings.GroupsInclusion.Excluded, IsFullName);
            FilterOut(_excludedStarts, AppSettings.GroupsInclusion.Excluded, EndsWith); // ends with after must start with
            FilterOut(_excludedEnds, AppSettings.GroupsInclusion.Excluded, StartsWith); // start with after must end with
            FilterOut(_excludedStartEnds, AppSettings.GroupsInclusion.Excluded, StartsEndsWith);
        }

        private static void FilterOut(TypeGroups destination, TypeGroups source, Func<string, bool> filter)
        {
            destination.Types = destination.Types.Union(source.Types.Where(filter).Select(x => x.Replace("*", ""))).ToList();
            destination.Namespaces = destination.Namespaces.Union(source.Namespaces.Where(filter).Select(x => x.Replace("*", ""))).ToList();
            destination.Assemblies = destination.Assemblies.Union(source.Assemblies.Where(filter).Select(x => x.Replace("*", ""))).ToList();
        }

        private static bool IsFullName(string x) => x.StartsWith("*") || !x.EndsWith("*");
        private static bool StartsWith(string x) => x.StartsWith("*") || !x.EndsWith("*");
        private static bool EndsWith(string x) => x.StartsWith("*") || !x.EndsWith("*");
        private static bool StartsEndsWith(string x) => x.StartsWith("*") || !x.EndsWith("*");
    }
}