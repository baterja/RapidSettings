using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;
using System.Linq;
using System.Threading.Tasks;

namespace RapidSettings.Tests.Attributes
{
    [TestClass]
    public class SanityTests
    {
        private class TestSettings
        {
            [ToFill]
            public int SomeSetting1 { get; private set; }

            [ToFill]
            public int? SomeNullableSetting2 { get; private set; }

            [ToFill("asdf", isRequired: false)]
            public int SomeNonConvertibleSetting { get; private set; }
        }

        [TestMethod]
        public void SanityTest()
        {
            var settingsFiller = this.GetSettingsFiller();

            var settings = settingsFiller.CreateWithSettings<TestSettings>();

            Assert.AreEqual(1, settings.SomeSetting1);
            Assert.AreEqual(2, settings.SomeNullableSetting2);
            Assert.AreEqual(default(int), settings.SomeNonConvertibleSetting);
        }

        [TestMethod]
        public async Task SanityTestAsync()
        {
            var settingsFiller = this.GetSettingsFiller();

            var settings = await settingsFiller.CreateWithSettingsAsync<TestSettings>();

            Assert.AreEqual(1, settings.SomeSetting1);
            Assert.AreEqual(2, settings.SomeNullableSetting2);
            Assert.AreEqual(default(int), settings.SomeNonConvertibleSetting);
        }

        private SettingsFiller GetSettingsFiller()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkStructsConverter() });
            var rawSettingsProvider = new FromFuncProvider(key => key.ToString().Last().ToString());
            var settingsFiller = new SettingsFiller(converterChooser, rawSettingsProvider);

            return settingsFiller;
        }
    }
}
