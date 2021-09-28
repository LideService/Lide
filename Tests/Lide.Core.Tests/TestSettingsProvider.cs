using System;
using System.Collections.Generic;
using Lide.Core.Model.Settings;
using Lide.Core.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Core.Tests
{
    [TestClass]
    public class TestSettingsProvider
    {
        [TestMethod]
        public void That_CanInitialize_When_JsonPropagateSettings()
        {
            var settingsProvider = new SettingsProvider(new SerializeProvider(), new CompressionProvider());
            var propagateSettings = new PropagateSettings()
            {
                InclusionPattern = "Test1",
                DecoratorsWithPattern = new List<string> { "Test2" },
                OverrideInclusionPattern = true,
                OverrideDecoratorsWithPattern = true,
            };

            settingsProvider.Initialize(new AppSettings(), System.Text.Json.JsonSerializer.Serialize(propagateSettings));
            Assert.AreEqual(ToBase64(propagateSettings), settingsProvider.PropagateSettingsString);
        }

        [TestMethod]
        public void That_CanInitialize_When_CompressedPropagateSettings()
        {
            var settingsProvider = new SettingsProvider(new SerializeProvider(), new CompressionProvider());
            var propagateSettings = new PropagateSettings()
            {
                InclusionPattern = "Test1",
                DecoratorsWithPattern = new List<string> { "Test2" },
                OverrideInclusionPattern = true,
                OverrideDecoratorsWithPattern = true,
            };

            settingsProvider.Initialize(new AppSettings(), ToBase64(propagateSettings));
            Assert.AreEqual(ToBase64(propagateSettings), settingsProvider.PropagateSettingsString);
        }

        [TestMethod]
        public void That_CanInitialize_When_NoPropagateSettings()
        {
            var settingsProvider = new SettingsProvider(new SerializeProvider(), new CompressionProvider());
            settingsProvider.Initialize(new AppSettings(), null);

            Assert.AreEqual(ToBase64(new PropagateSettings()), settingsProvider.PropagateSettingsString);
        }

        [TestMethod]
        public void That_AllowReadonlyDecorators_IsTrue_WhenBothEnabledKeysMatch()
        {
            var settingsProvider = new SettingsProvider(new SerializeProvider(), new CompressionProvider());

            void AssertAllowReadonlyWithKeys(bool expected, string appEnabledKey, string propagateEnabledKey)
            {
                settingsProvider.Initialize(new AppSettings() { EnabledKey = appEnabledKey }, ToBase64(new PropagateSettings() { EnabledKey = propagateEnabledKey }));
                Assert.AreEqual(expected, settingsProvider.AllowReadonlyDecorators);
            }

            AssertAllowReadonlyWithKeys(true, string.Empty, "WillWorkWithAnythingAsBaseKeyIsNotSet");
            AssertAllowReadonlyWithKeys(true, string.Empty, string.Empty);
            AssertAllowReadonlyWithKeys(false, "WillNotWorkWithoutMatchingPropagateKey", string.Empty);
            AssertAllowReadonlyWithKeys(false, "WillNotWorkWithoutMatchingPropagateKey", "KeysDontMatch");
            AssertAllowReadonlyWithKeys(true, "MatchingKeys", "MatchingKeys");
        }

        [TestMethod]
        public void That_AllowVolatileDecorators_IsTrue_WhenAllKeysMatch()
        {
            var settingsProvider = new SettingsProvider(new SerializeProvider(), new CompressionProvider());

            void AssertAllowReadonlyWithKeys(bool expected, string appEnabledKey, string propagateEnabledKey, string appVolatileKey, string propagateVolatileKey)
            {
                settingsProvider.Initialize(new AppSettings() { EnabledKey = appEnabledKey, VolatileKey = appVolatileKey },
                    ToBase64(new PropagateSettings() { EnabledKey = propagateEnabledKey, VolatileKey = propagateVolatileKey }));
                Assert.AreEqual(expected, settingsProvider.AllowVolatileDecorators);
            }

            AssertAllowReadonlyWithKeys(true, string.Empty, string.Empty, string.Empty, "WillWorkWithAnythingAsBaseKeyIsNotSet");
            AssertAllowReadonlyWithKeys(true, string.Empty, string.Empty, string.Empty, string.Empty);
            AssertAllowReadonlyWithKeys(false, string.Empty, string.Empty, "WillNotWorkWithoutMatchingPropagateKey", string.Empty);
            AssertAllowReadonlyWithKeys(false, string.Empty, string.Empty, "WillNotWorkWithoutMatchingPropagateKey", "KeysDontMatch");
            AssertAllowReadonlyWithKeys(true, string.Empty, string.Empty, "MatchingKeys", "MatchingKeys");
            AssertAllowReadonlyWithKeys(false, "NotMatchingKey", string.Empty, "MatchingKeys", "MatchingKeys");
            AssertAllowReadonlyWithKeys(true, "MatchingEnabled", "MatchingEnabled", "MatchingKeys", "MatchingKeys");
        }

        [TestMethod]
        public void That_GetDecorators_ReturnProperSet_When_DifferentPatterns()
        {
            var settingsProvider = new SettingsProvider(new SerializeProvider(), new CompressionProvider());
            var appSettings = new AppSettings() { DecoratorsWithPattern = new List<string> { "Decorator1" } };
            settingsProvider.Initialize(appSettings, null);
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { DecoratorsWithPattern = new List<string> { "Decorator1+Newtonsoft.*" } };
            settingsProvider.Initialize(appSettings, null);
            Assert.AreEqual(1, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { InclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1" } };
            settingsProvider.Initialize(appSettings, null);
            Assert.AreEqual(1, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { InclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1" } };
            settingsProvider.Initialize(appSettings, null);
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(TestSettingsProvider)).Count);
            appSettings = new AppSettings() { InclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1-Newtonsoft.Json.JsonConvert" } };
            settingsProvider.Initialize(appSettings, null);
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { InclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1-Newtonsoft.Json" } };
            settingsProvider.Initialize(appSettings, null);
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { InclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1-Newtonsoft.*" } };
            settingsProvider.Initialize(appSettings, null);
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { InclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1-Newtonsoft.*+Newtonsoft.Json.JsonConvert" } };
            settingsProvider.Initialize(appSettings, null);
            Assert.AreEqual(1, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonReader)).Count);
            
        }

        private static string ToBase64(PropagateSettings propagateSettings)
        {
            var serializer = new SerializeProvider();
            var compressor = new CompressionProvider();
            var serialized = serializer.Serialize(propagateSettings);
            var compressed = compressor.Compress(serialized);
            var base64 = Convert.ToBase64String(compressed);
            return base64;
        }
    }
}