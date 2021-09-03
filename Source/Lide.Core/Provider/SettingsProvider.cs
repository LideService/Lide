using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Model.Settings;

namespace Lide.Core.Provider
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly ISerializerFacade _serializerFacade;
        private readonly ICompressionProvider _compressionProvider;
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
            "System.",
        };

        public SettingsProvider(
            ISerializerFacade serializerFacade,
            ICompressionProvider compressionProvider)
        {
            _serializerFacade = serializerFacade;
            _compressionProvider = compressionProvider;
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

        public bool SearchHttpBody => AppSettings.SearchHttpBody;
        public bool AllowVolatileDecorators => string.IsNullOrEmpty(AppSettings.EnabledKey) || AppSettings.VolatileKey == PropagateSettings.VolatileKey;
        public bool AllowDecoratorsKeyMatch => string.IsNullOrEmpty(AppSettings.EnabledKey) || AppSettings.EnabledKey == PropagateSettings.EnabledKey;

        public void SetData(AppSettings appSettings, string propagateSettings)
        {
            AppSettings = appSettings;
            PropagateSettings = DeserializeSafe(propagateSettings);
            var serialized = _serializerFacade.Serialize(PropagateSettings);
            var compressed = _compressionProvider.Compress(serialized);
            PropagateSettingsString = Convert.ToBase64String(compressed);
            BuildIncludedTypes();
            BuildExcludedTypes();
            BuildDecorators();
        }

        public List<string> GetDecorators() => _decorators;

        public bool IsTypeAllowed(Type type)
        {
            var isIncludedType = IsIncluded(type.Name, x => x.Types);
            var isIncludedNamespace = IsIncluded(type.Namespace ?? string.Empty, x => x.Namespaces);
            var isIncludedAssemblies = IsIncluded(type.Assembly.GetName().Name ?? string.Empty, x => x.Assemblies);

            var isExcludedType = IsExcluded(type.Name, x => x.Types);
            var isExcludedNamespace = IsExcluded(type.Namespace ?? string.Empty, x => x.Namespaces);
            var isExcludedAssemblies = IsExcluded(type.Assembly.GetName().Name ?? string.Empty, x => x.Assemblies);

            var isIncluded = isIncludedType || isIncludedNamespace || isIncludedAssemblies;
            var isExcluded = isExcludedType || isExcludedNamespace || isExcludedAssemblies;

            var isAllowed = AppSettings.GroupsInclusion.InclusionType switch
            {
                InclusionType.OnlyIncluded => isIncluded,
                InclusionType.AllButExcluded => !isExcluded,
                _ => !isExcluded || isIncluded
            };

            return isAllowed;
        }

        private bool IsIncluded(string name, Func<TypeGroups, List<string>> selector)
        {
            return selector(_includedFullname).Any(x => x == name)
                   || selector(_includedStarts).Any(name.StartsWith)
                   || selector(_includedEnds).Any(name.EndsWith)
                   || selector(_includedStartEnds).Any(x => name.StartsWith(x) && name.EndsWith(x));
        }

        private bool IsExcluded(string name, Func<TypeGroups, List<string>> selector)
        {
            return selector(_excludedFullname).Any(x => x == name)
                   || selector(_excludedStarts).Any(name.StartsWith)
                   || selector(_excludedEnds).Any(name.EndsWith)
                   || selector(_excludedStartEnds).Any(x => name.StartsWith(x) && name.EndsWith(x));
        }

        private void BuildDecorators()
        {
            _decorators = PropagateSettings.Decorators;
            if (PropagateSettings.OverrideDecorators)
            {
                return;
            }

            _decorators = _decorators.Union(AppSettings.Decorators).ToList();
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

        private PropagateSettings DeserializeSafe(string propagateSettings, bool useCompression = false, bool returnDefault = false)
        {
            try
            {
                if (useCompression)
                {
                    var compressed = Convert.FromBase64String(propagateSettings);
                    var decompressed = _compressionProvider.Decompress(compressed);
                    return _serializerFacade.Deserialize<PropagateSettings>(decompressed);
                }

                var data = Encoding.UTF8.GetBytes(propagateSettings);
                return _serializerFacade.Deserialize<PropagateSettings>(data);
            }
            catch
            {
                return returnDefault
                    ? new PropagateSettings()
                    : DeserializeSafe(propagateSettings, true, true);
            }
        }
    }
}