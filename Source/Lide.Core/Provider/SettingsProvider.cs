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
        private readonly ISerializeProvider _serializeProvider;
        private readonly ICompressionProvider _compressionProvider;
        private readonly List<(bool inclusion, string pattern)> _globalPatterns = new ();
        private readonly List<(bool inclusion, string pattern)> _defaultPatterns = new ();
        private readonly Dictionary<string, List<(bool inclusion, string pattern)>> _decoratorPatterns = new ();
        private PropagateSettings _propagateSettings;

        public SettingsProvider(
            ISerializeProvider serializeProvider,
            ICompressionProvider compressionProvider)
        {
            _serializeProvider = serializeProvider;
            _compressionProvider = compressionProvider;
        }

        public AppSettings AppSettings { get; private set; }
        public string PropagateSettingsString { get; private set; }
        public bool AllowVolatileDecorators => AllowReadonlyDecorators && (string.IsNullOrEmpty(AppSettings.VolatileKey) || AppSettings.VolatileKey == _propagateSettings.VolatileKey);
        public bool AllowReadonlyDecorators => string.IsNullOrEmpty(AppSettings.EnabledKey) || AppSettings.EnabledKey == _propagateSettings.EnabledKey;

        public void Initialize(AppSettings appSettings, string propagateSettings)
        {
            AppSettings = appSettings;
            PreparePropagateSettings(propagateSettings);
            PreparePatterns();
        }

        public List<string> GetDecorators(Type type)
        {
            var result = new List<string>();
            var globalInclusion = IsIncludedInPattern(type, _globalPatterns);
            var defaultInclusion = IsIncludedInPattern(type, _defaultPatterns);
            if (defaultInclusion.HasValue && !defaultInclusion.Value)
            {
                return result;
            }

            foreach (var (decoratorName, decoratorPatterns) in _decoratorPatterns)
            {
                var decoratorInclusion = IsIncludedInPattern(type, decoratorPatterns);
                if (decoratorInclusion ?? globalInclusion ?? false)
                {
                    result.Add(decoratorName);
                }
            }

            return result;
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

        private void PreparePropagateSettings(string propagateSettings)
        {
            try
            {
                var fromBase64 = Convert.FromBase64String(propagateSettings);
                var decompressed = _compressionProvider.Decompress(fromBase64);
                var deserialized = _serializeProvider.Deserialize<PropagateSettings>(decompressed);
                _propagateSettings = deserialized;
                PropagateSettingsString = propagateSettings;
            }
            catch
            {
                try
                {
                    _propagateSettings = _serializeProvider.DeserializeFromString<PropagateSettings>(propagateSettings);
                }
                catch
                {
                    _propagateSettings = new PropagateSettings();
                }

                var serialized = _serializeProvider.Serialize(_propagateSettings);
                var compressed = _compressionProvider.Compress(serialized);
                var toBase64 = Convert.ToBase64String(compressed);
                PropagateSettingsString = toBase64;
            }
        }

        private void PreparePatterns()
        {
            _defaultPatterns.AddRange(GetInclusionPatterns(ExcludedByDefault).Item2);
            if (!_propagateSettings.OverrideInclusionPattern)
            {
                _globalPatterns.AddRange(GetInclusionPatterns(AppSettings.InclusionPattern).Item2);
            }

            _globalPatterns.AddRange(GetInclusionPatterns(_propagateSettings.InclusionPattern).Item2);

            if (!_propagateSettings.OverrideDecoratorsWithPattern)
            {
                foreach (var decoratorWithPattern in AppSettings.DecoratorsWithPattern.Where(x => x.Length > 0))
                {
                    var (name, patterns) = GetInclusionPatterns(decoratorWithPattern);
                    _decoratorPatterns.TryAdd(name, new List<(bool, string)>());
                    _decoratorPatterns[name].AddRange(patterns);
                }
            }

            foreach (var decoratorWithPattern in _propagateSettings.DecoratorsWithPattern.Where(x => x.Length > 0))
            {
                var (name, patterns) = GetInclusionPatterns(decoratorWithPattern);
                _decoratorPatterns.TryAdd(name, new List<(bool, string)>());
                _decoratorPatterns[name].AddRange(patterns);
            }
        }

        private (string, List<(bool, string)>) GetInclusionPatterns(string inclusionPattern)
        {
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