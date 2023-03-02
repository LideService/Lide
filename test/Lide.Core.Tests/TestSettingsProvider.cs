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
        public void That_AllowReadonlyDecorators_IsTrue_WhenBothEnabledKeysMatch()
        {
            var settingsProvider = new SettingsProvider();

            void AssertAllowReadonlyWithKeys(bool expected, string appEnabledKey, string propagateEnabledKey)
            {
                settingsProvider.Initialize(
                    new AppSettings() { EnabledKey = appEnabledKey },
                    new PropagateSettings() { EnabledKey = propagateEnabledKey },
                    "");
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
            var settingsProvider = new SettingsProvider();

            void AssertAllowReadonlyWithKeys(bool expected, string appEnabledKey, string propagateEnabledKey, string appVolatileKey, string propagateVolatileKey)
            {
                settingsProvider.Initialize(
                    new AppSettings() { EnabledKey = appEnabledKey, VolatileKey = appVolatileKey },
                    new PropagateSettings() { EnabledKey = propagateEnabledKey, VolatileKey = propagateVolatileKey },
                    "");
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
            var settingsProvider = new SettingsProvider();
            var appSettings = new AppSettings() { DecoratorsWithPattern = new List<string> { "Decorator1" } };
            settingsProvider.Initialize(appSettings, new PropagateSettings(), "");
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { DecoratorsWithPattern = new List<string> { "Decorator1+Newtonsoft.*" } };
            settingsProvider.Initialize(appSettings, new PropagateSettings(), "");
            Assert.AreEqual(1, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { TypesInclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1" } };
            settingsProvider.Initialize(appSettings, new PropagateSettings(), "");
            Assert.AreEqual(1, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { TypesInclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1" } };
            settingsProvider.Initialize(appSettings, new PropagateSettings(), "");
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(TestSettingsProvider)).Count);
            appSettings = new AppSettings() { TypesInclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1-Newtonsoft.Json.JsonConvert" } };
            settingsProvider.Initialize(appSettings, new PropagateSettings(), "");
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { TypesInclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1-Newtonsoft.Json" } };
            settingsProvider.Initialize(appSettings, new PropagateSettings(), "");
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { TypesInclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1-Newtonsoft.*" } };
            settingsProvider.Initialize(appSettings, new PropagateSettings(), "");
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            appSettings = new AppSettings() { TypesInclusionPattern = "+*", DecoratorsWithPattern = new List<string> { "Decorator1-Newtonsoft.*+Newtonsoft.Json.JsonConvert" } };
            settingsProvider.Initialize(appSettings, new PropagateSettings(), "");
            Assert.AreEqual(1, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonConvert)).Count);
            Assert.AreEqual(0, settingsProvider.GetDecorators(typeof(Newtonsoft.Json.JsonReader)).Count);
        }
    }
}