#if !NETCOREAPP2_0 // NET Core AppSettings incorrectly gets config file in tests
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;

namespace RapidSettings.Tests.Providers
{
    [TestClass]
    public class FromAppSettingsProviderTests
    {
        [TestMethod]
        public void FromAppSettingsProviderTest_SimpleRetrieval()
        {
            var fromAppSettingsProvider = new FromAppSettingsProvider();

            var existingKeyValue = fromAppSettingsProvider.GetRawSetting("ExistingKey");

            Assert.AreEqual("ExistingKeyValue", existingKeyValue);
        }

        [TestMethod]
        public void FromAppSettingsProviderTest_NonExistingRetrieval()
        {
            var fromAppSettingsProvider = new FromAppSettingsProvider();

            var notExistingKeyValue = fromAppSettingsProvider.GetRawSetting("NotExistingKey");

            Assert.IsNull(notExistingKeyValue, "Not existing key value is not null!");
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException), AllowDerivedTypes = true)]
        public void FromAppSettingsProviderTest_NullRetrieval()
        {
            var fromAppSettingsProvider = new FromAppSettingsProvider();

            var nullKeyValue = fromAppSettingsProvider.GetRawSetting(null);
        }
    }
}
#endif