using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;

namespace RapidSettings.Tests.Providers
{
    [TestClass]
    public class FromAppSettingsProviderTests
    {
        [TestMethod]
        public void SimpleRetrieval()
        {
            var fromAppSettingsProvider = new FromAppSettingsProvider();

            var existingKeyValue = fromAppSettingsProvider.GetRawSetting("ExistingKey");

            Assert.AreEqual("ExistingKeyValue", existingKeyValue);
        }

        [TestMethod]
        public void NonExistingRetrieval()
        {
            var fromAppSettingsProvider = new FromAppSettingsProvider();

            var notExistingKeyValue = fromAppSettingsProvider.GetRawSetting("NotExistingKey");

            Assert.IsNull(notExistingKeyValue, "Not existing key value is not null!");
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException), AllowDerivedTypes = true)]
        public void NullRetrieval()
        {
            var fromAppSettingsProvider = new FromAppSettingsProvider();

            _ = fromAppSettingsProvider.GetRawSetting(null);
        }
    }
}
