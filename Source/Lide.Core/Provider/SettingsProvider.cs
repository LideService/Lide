using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lide.Core.Contract.Provider;
using Lide.Core.Model.Settings;

namespace Lide.Core.Provider
{
    public class SettingsProvider : ISettingsProvider
    {
        private const string ExcludedByDefault = "-Lide.*-Microsoft.*-System.*";
        private readonly List<(bool inclusion, string pattern)> _globalTypesPatterns = new ();
        private readonly List<(bool inclusion, string pattern)> _defaultTypesPatterns = new ();
        private readonly List<(bool inclusion, string pattern)> _globalAddressesPatterns = new ();
        private readonly Dictionary<string, List<(bool inclusion, string pattern)>> _decoratorPatterns = new ();

        public int Depth { get; set; }
        public int NextDepth => Depth + 1;
        public AppSettings AppSettings { get; private set; }
        public PropagateSettings PropagateSettings { get; private set; }
        public string OriginRequestPath { get; set; }
        public bool AllowVolatileDecorators => AllowReadonlyDecorators && (string.IsNullOrEmpty(AppSettings.VolatileKey) || AppSettings.VolatileKey == PropagateSettings.VolatileKey);
        public bool AllowReadonlyDecorators => string.IsNullOrEmpty(AppSettings.EnabledKey) || AppSettings.EnabledKey == PropagateSettings.EnabledKey;

        public void Initialize(AppSettings appSettings, PropagateSettings propagateSettings)
        {
            AppSettings = appSettings;
            PropagateSettings = propagateSettings;
            PreparePatterns();
        }

        public bool IsDecoratorIncluded(string decoratorName)
        {
            return _decoratorPatterns.ContainsKey(decoratorName);
        }

        public bool IsTypeDisallowed(Type type)
        {
            var globalInclusion = IsIncludedInPattern(type, _globalTypesPatterns);
            var defaultInclusion = IsIncludedInPattern(type, _defaultTypesPatterns);
            return globalInclusion == false || defaultInclusion == false;
        }

        public ISet<string> GetDecorators(Type type)
        {
            var result = new HashSet<string>();
            var globalInclusion = IsIncludedInPattern(type, _globalTypesPatterns);
            var defaultInclusion = IsIncludedInPattern(type, _defaultTypesPatterns);
            if (globalInclusion == false || defaultInclusion == false)
            {
                return result;
            }

            foreach (var (decoratorName, decoratorPatterns) in _decoratorPatterns)
            {
                var decoratorInclusion = IsIncludedInPattern(type, decoratorPatterns);
                if (decoratorInclusion ?? globalInclusion ?? AppSettings.DefaultTypeInclusion)
                {
                    result.Add(decoratorName);
                }
            }

            return result;
        }

        public bool IsAddressAllowed(string address)
        {
            bool? included = null;
            foreach (var (inclusion, pattern) in _globalAddressesPatterns)
            {
                if (included == inclusion)
                {
                    continue;
                }

                if (Regex.IsMatch(address, pattern))
                {
                    included = inclusion;
                }
            }

            return included ?? AppSettings.DefaultAddressInclusion;
        }

        private bool? IsIncludedInPattern(Type type, List<(bool inclusion, string pattern)> inclusionPatterns)
        {
            bool? included = null;
            var typeFullname = type.FullName ?? string.Empty;
            var typeNamespace = type.Namespace ?? string.Empty;
            var typeAssembly = type.Assembly.GetName().Name ?? string.Empty;
            foreach (var (inclusion, pattern) in inclusionPatterns)
            {
                if (included == inclusion)
                {
                    continue;
                }

                if (Regex.IsMatch(type.Name, pattern) || Regex.IsMatch(typeFullname, pattern)
                    || Regex.IsMatch(typeNamespace, pattern) || Regex.IsMatch(typeAssembly, pattern))
                {
                    included = inclusion;
                }
            }

            return included;
        }

        private void PreparePatterns()
        {
            _defaultTypesPatterns.AddRange(GetInclusionPatterns(ExcludedByDefault).Item2);
            _globalTypesPatterns.AddRange(GetInclusionPatterns(PropagateSettings.TypesInclusionPattern).Item2);
            if (!PropagateSettings.OverrideInclusionPattern)
            {
                _globalTypesPatterns.AddRange(GetInclusionPatterns(AppSettings.TypesInclusionPattern).Item2);
            }

            foreach (var decoratorWithPattern in PropagateSettings.DecoratorsWithPattern.Where(x => x?.Length > 0))
            {
                var (name, patterns) = GetInclusionPatterns(decoratorWithPattern);
                _decoratorPatterns.TryAdd(name, new List<(bool, string)>());
                _decoratorPatterns[name].AddRange(patterns);
            }

            if (!PropagateSettings.OverrideDecoratorsWithPattern)
            {
                foreach (var decoratorWithPattern in AppSettings.DecoratorsWithPattern.Where(x => x?.Length > 0))
                {
                    var (name, patterns) = GetInclusionPatterns(decoratorWithPattern);
                    _decoratorPatterns.TryAdd(name, new List<(bool, string)>());
                    _decoratorPatterns[name].AddRange(patterns);
                }
            }

            _globalAddressesPatterns.AddRange(GetInclusionPatterns(PropagateSettings.AddressesInclusionPattern).Item2);
            if (!PropagateSettings.OverrideAddressesPattern)
            {
                _globalAddressesPatterns.AddRange(GetInclusionPatterns(AppSettings.AddressesInclusionPattern).Item2);
            }
        }

        private (string, List<(bool, string)>) GetInclusionPatterns(string inclusionPattern)
        {
            if (string.IsNullOrEmpty(inclusionPattern))
            {
                return (string.Empty, new List<(bool, string)>());
            }

            var result = new List<(bool, string)>();
            var globalInclusionNames = Regex.Matches(inclusionPattern, "([-+]?[^+-]*)")
                .Select(x => x.Value)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var first = globalInclusionNames.FirstOrDefault(x => !x.StartsWith('+') && !x.StartsWith('-'));
            foreach (var inclusionName in globalInclusionNames.Where(x => x.StartsWith('+') || x.StartsWith('-')))
            {
                var inclusion = inclusionName.StartsWith('+');
                var pattern = WildCardToRegex(inclusionName[1..]);
                result.Add((inclusion, pattern));
            }

            return (first, result);
        }

        private static string WildCardToRegex(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }
    }
}