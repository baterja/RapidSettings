using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidSettings.Tests
{
    [TestClass]
    public class ClassToFillAttributeTests
    {
        [ClassToFill]
        private class DefaultClassToFillSettings
        {
            public int SomeSetting1 { get; private set; }

            [ToFill("asdf", isRequired: false)]
            public int SomeNotRetrievableSetting { get; private set; }
        }

        [TestMethod]
        public void ShouldFillPropertiesAccordingToDefaultRules()
        {
            var settingsFiller = GetBasicSettingsFiller();

            var settings = settingsFiller.CreateWithSettings<DefaultClassToFillSettings>();

            Assert.AreEqual(1, settings.SomeSetting1);
            Assert.AreEqual(default(int), settings.SomeNotRetrievableSetting);
        }

        [ClassToFill("prefix", allRequired: false, rawSettingsProviderName: "func")]
        private class CustomClassToFillSettings
        {
            public int SomeSetting1 { get; private set; }

            [ToFill("unprefixed", isRequired: true, rawSettingsProviderName: "otherFunc")]
            public int SomeOtherSetting{ get; private set; }
        }

        [TestMethod]
        public void ShouldFillPropertiesAccordingToCustomizedRules()
        {
            var funcSettingsProvider = new FromFuncProvider(key => key.Last().ToString());
            var otherFuncSettingsProvider = new FromFuncProvider(_ => 100);
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });
            var settingsFiller = new SettingsFiller(
                converterChooser,
                new Dictionary<string, IRawSettingsProvider>
                {
                    { "func", funcSettingsProvider },
                    { "otherFunc", otherFuncSettingsProvider },
                },
                funcSettingsProvider);

            var settings = settingsFiller.CreateWithSettings<CustomClassToFillSettings>();

            Assert.AreEqual(1, settings.SomeSetting1);
            Assert.AreEqual(100, settings.SomeOtherSetting);
        }

        // TODO share with SettingsFillerTests
        private static SettingsFiller GetBasicSettingsFiller()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });
            var rawSettingsProvider = new FromFuncProvider(key => key.Last().ToString());
            var settingsFiller = new SettingsFiller(converterChooser, rawSettingsProvider);

            return settingsFiller;
        }
    }
}
