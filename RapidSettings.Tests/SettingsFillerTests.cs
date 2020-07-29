using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidSettings.Tests.Attributes
{
    [TestClass]
    public class SettingsFillerTests
    {
        #region Basics
        private class BasicSettings
        {
            [ToFill]
            public int SomeSetting1 { get; private set; }

            [ToFill]
            public int? SomeNullableSetting2 { get; private set; }

            [ToFill("asdf", isRequired: false)]
            public int SomeNonConvertibleSetting { get; private set; }
        }

        [TestMethod]
        public void SettingsFillerTest_NoProvider()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });
            var settingsFiller = new SettingsFiller(converterChooser, (IRawSettingsProvider)null);

            foreach (var settingsProviderWithKey in SettingsFillerStaticDefaults.DefaultRawSettingsProviders)
            {
                Assert.IsTrue(settingsFiller.RawSettingsProvidersByNames.ContainsKey(settingsProviderWithKey.Key));
                Assert.AreEqual(settingsFiller.RawSettingsProvidersByNames[settingsProviderWithKey.Key], settingsProviderWithKey.Value);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException))]
        public void SettingsFillerTest_NoProviders()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });
            var settingsFiller = new SettingsFiller(converterChooser, (IDictionary<string, IRawSettingsProvider>)null);
        }

        [TestMethod]
        public void SettingsFillerTest_NoChooser()
        {
            var rawSettingsProvider = new FromFuncProvider(key => key.ToString().Last().ToString());
            var settingsFiller = new SettingsFiller(null, rawSettingsProvider);

            Assert.AreEqual(SettingsFillerStaticDefaults.DefaultSettingsConverterChooser, settingsFiller.SettingsConverterChooser);
        }

        [TestMethod]
        public void SettingsFillerTest_Basic()
        {
            var settingsFiller = this.GetBasicSettingsFiller();

            var settings = settingsFiller.CreateWithSettings<BasicSettings>();

            Assert.AreEqual(1, settings.SomeSetting1);
            Assert.AreEqual(2, settings.SomeNullableSetting2);
            Assert.AreEqual(default(int), settings.SomeNonConvertibleSetting);
        }

        private class SettingsWithUnretrievableProp
        {
            [ToFill("badKey")]
            public int SomeSetting1 { get; private set; }
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException))]
        public void SettingsFillerTest_NullResolutionOfRequired()
        {
            var settingsFiller = this.GetBasicSettingsFiller();

            var settings = settingsFiller.CreateWithSettings<SettingsWithUnretrievableProp>();
        }

        private class SettingsWithoutSetterOnProperty
        {
            [ToFill("whatever")]
            public int SomeSetting1 { get; }
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException))]
        public void SettingsFillerTest_NoSetterForDecoratedProperty()
        {
            var settingsFiller = this.GetBasicSettingsFiller();

            var settings = settingsFiller.CreateWithSettings<SettingsWithoutSetterOnProperty>();
        }


        private SettingsFiller GetBasicSettingsFiller()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });
            var rawSettingsProvider = new FromFuncProvider(key => key.ToString().Last().ToString());
            var settingsFiller = new SettingsFiller(converterChooser, rawSettingsProvider);

            return settingsFiller;
        }
        #endregion

        #region Advanced scenarios
        [TestMethod]
        public void SettingsFillerTest_Advanced()
        {
            var settingsFiller = this.GetAdvancedSettingsFiller();
            Environment.SetEnvironmentVariable(nameof(AdvancedSettings.SomeInt), "1");

            var settings = settingsFiller.CreateWithSettings<AdvancedSettings>();

            Assert.IsNotNull(settings.SomeA);
            Assert.IsNotNull(settings.SomeB);
            Assert.IsNotNull(settings.SomeC);
            Assert.AreEqual(1, settings.SomeInt);
            Assert.IsNull(settings.SomeOptionalInt);
        }

        private class AdvancedSettings
        {
            [ToFill]
            public A SomeA { get; private set; }

            [ToFill]
            public B SomeB { get; private set; }

            [ToFill]
            public C SomeC { get; private set; }

            [ToFill(rawSettingsProviderName: "env")]
            public int SomeInt { get; private set; }

            [ToFill(isRequired: false, rawSettingsProviderName: "env")]
            public int? SomeOptionalInt { get; private set; }
        }

        private class A { }
        private class B : A { }
        private class C : B { }

        private class SuperConverter : RawSettingsConverterBase
        {
            public SuperConverter()
            {
                this.AddSupportForTypes(typeof(A), typeof(A), (rawValue, type) => rawValue);
                this.AddSupportForTypes(typeof(C), typeof(C), (rawValue, type) => rawValue);
            }
        }

        private SettingsFiller GetAdvancedSettingsFiller()
        {
            var converterChooser = new SettingsConverterChooser(new IRawSettingsConverter[] { new StringToFrameworkTypesConverter(), new SuperConverter() });
            var funcSettingsProvider = new FromFuncProvider(key => new C());
            var envSettingsProvider = new FromEnvironmentProvider();
            var settingsFiller = new SettingsFiller(converterChooser, new Dictionary<string, IRawSettingsProvider> { { "func", funcSettingsProvider }, { "env", envSettingsProvider } }, funcSettingsProvider);

            return settingsFiller;
        }
        #endregion
    }
}
