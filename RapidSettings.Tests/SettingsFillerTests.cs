using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Converters;
using RapidSettings.Core;
using RapidSettings.Exceptions;
using RapidSettings.Interfaces;
using RapidSettings.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [ExpectedException(typeof(RapidSettingsException))]
        public void SettingsFillerTest_NoProvider()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkStructsConverter() });
          
            var settingsFiller = new SettingsFiller(converterChooser, (IRawSettingsProvider)null);
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException))]
        public void SettingsFillerTest_NoProviders()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkStructsConverter() });
            var settingsFiller = new SettingsFiller(converterChooser, (IDictionary<string, IRawSettingsProvider>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException))]
        public void SettingsFillerTest_NoChooser()
        {
            var rawSettingsProvider = new FromFuncProvider(key => key.ToString().Last().ToString());
            var settingsFiller = new SettingsFiller(null, rawSettingsProvider);
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

        [TestMethod]
        public async Task SettingsFillerTest_BasicAsync()
        {
            var settingsFiller = this.GetBasicSettingsFiller();

            var settings = await settingsFiller.CreateWithSettingsAsync<BasicSettings>();

            Assert.AreEqual(1, settings.SomeSetting1);
            Assert.AreEqual(2, settings.SomeNullableSetting2);
            Assert.AreEqual(default(int), settings.SomeNonConvertibleSetting);
        }

        private SettingsFiller GetBasicSettingsFiller()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkStructsConverter() });
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
            Environment.SetEnvironmentVariable(nameof(AdvancedSettings.SomeWrappedInt), "2");
            Environment.SetEnvironmentVariable(nameof(AdvancedSettings.SomeOptionalWrappedRetrievableInt), "3");

            var settings = settingsFiller.CreateWithSettings<AdvancedSettings>();

            Assert.IsNotNull(settings.SomeA);
            Assert.IsNotNull(settings.SomeB);
            Assert.IsNotNull(settings.SomeC);
            Assert.AreEqual(1, settings.SomeInt);
            Assert.IsNull(settings.SomeOptionalInt);

            Assert.IsTrue(settings.SomeWrappedInt.Metadata.HasValueSpecified);
            Assert.IsTrue(settings.SomeWrappedInt.Metadata.IsRequired);
            Assert.AreEqual(nameof(AdvancedSettings.SomeWrappedInt), settings.SomeWrappedInt.Metadata.Key);
            Assert.AreEqual(2, settings.SomeWrappedInt.Value);

            Assert.IsFalse(settings.SomeOptionalWrappedNonRetrievableInt.Metadata.HasValueSpecified);
            Assert.IsFalse(settings.SomeOptionalWrappedNonRetrievableInt.Metadata.IsRequired);
            Assert.AreEqual(nameof(AdvancedSettings.SomeOptionalWrappedNonRetrievableInt), settings.SomeOptionalWrappedNonRetrievableInt.Metadata.Key);
            Assert.AreEqual(default(int), settings.SomeOptionalWrappedNonRetrievableInt.Value);

            Assert.IsTrue(settings.SomeOptionalWrappedRetrievableInt.Metadata.HasValueSpecified);
            Assert.IsFalse(settings.SomeOptionalWrappedRetrievableInt.Metadata.IsRequired);
            Assert.AreEqual(nameof(AdvancedSettings.SomeOptionalWrappedRetrievableInt), settings.SomeOptionalWrappedRetrievableInt.Metadata.Key);
            Assert.AreEqual(3, settings.SomeOptionalWrappedRetrievableInt.Value);

            Assert.IsFalse(settings.SomeOptionalWrappedNullableNonRetrievableInt.Metadata.HasValueSpecified);
            Assert.IsFalse(settings.SomeOptionalWrappedNullableNonRetrievableInt.Metadata.IsRequired);
            Assert.AreEqual(nameof(AdvancedSettings.SomeOptionalWrappedNullableNonRetrievableInt), settings.SomeOptionalWrappedNullableNonRetrievableInt.Metadata.Key);
            Assert.IsNull(settings.SomeOptionalWrappedNullableNonRetrievableInt.Value);
        }

        private class AdvancedSettings
        {
            [ToFill(rawSettingsProviderName: "func")]
            public A SomeA { get; private set; }

            [ToFill(rawSettingsProviderName: "func")]
            public B SomeB { get; private set; }

            [ToFill(rawSettingsProviderName: "func")]
            public C SomeC { get; private set; }

            [ToFill(rawSettingsProviderName: "env")]
            public int SomeInt { get; private set; }

            [ToFill(rawSettingsProviderName: "env")]
            public Setting<int> SomeWrappedInt { get; private set; }

            [ToFill(isRequired: false, rawSettingsProviderName: "env")]
            public int? SomeOptionalInt { get; private set; }

            [ToFill(isRequired: false, rawSettingsProviderName: "env")]
            public Setting<int> SomeOptionalWrappedRetrievableInt { get; private set; }

            [ToFill(isRequired: false, rawSettingsProviderName: "env")]
            public Setting<int> SomeOptionalWrappedNonRetrievableInt { get; private set; }

            [ToFill(isRequired: false, rawSettingsProviderName: "env")]
            public Setting<int?> SomeOptionalWrappedNullableNonRetrievableInt { get; private set; }
        }

        private class A { }
        private class B : A { }
        private class C : B { }

        private class SuperConverter : RawSettingsConverterBase
        {
            public SuperConverter()
            {
                AddSupportForTypes(typeof(A), typeof(A), (rawValue, type) => rawValue);
                AddSupportForTypes(typeof(C), typeof(C), (rawValue, type) => rawValue);
            }
        }

        private SettingsFiller GetAdvancedSettingsFiller()
        {
            var converterChooser = new SettingsConverterChooser(new IRawSettingsConverter[] { new StringToFrameworkStructsConverter(), new SuperConverter() });
            var funcSettingsProvider = new FromFuncProvider(key => new C());
            var envSettingsProvider = new FromEnvironmentProvider();
            var settingsFiller = new SettingsFiller(converterChooser, new Dictionary<string, IRawSettingsProvider> { { "func", funcSettingsProvider }, { "env", envSettingsProvider } });

            return settingsFiller;
        }
        #endregion
    }
}
