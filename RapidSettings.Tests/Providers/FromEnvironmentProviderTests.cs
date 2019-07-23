using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;
using System;

namespace RapidSettings.Tests.Providers
{
    [TestClass]
    public class FromEnvironmentProviderTests
    {
        [TestMethod]
        public void FromEnvironmentProviderTest_SimpleRetrieval()
        {
            var fromEnvironmentProvider = new FromEnvironmentProvider();
            Environment.SetEnvironmentVariable("ExistingKey", "ExistingKeyValue");

            var existingKeyValue = fromEnvironmentProvider.GetRawSetting("ExistingKey");

            Assert.AreEqual("ExistingKeyValue", existingKeyValue);
        }

        [TestMethod]
        public void FromEnvironmentProviderTest_NonExistingRetrieval()
        {
            var fromEnvironmentProvider = new FromEnvironmentProvider();
            Environment.SetEnvironmentVariable("ExistingKey", "ExistingKeyValue");

            var notExistingKeyValue = fromEnvironmentProvider.GetRawSetting("NotExistingKey");

            Assert.IsNull(notExistingKeyValue, "Not existing key value is not null!");
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException), AllowDerivedTypes = true)]
        public void FromEnvironmentProviderTest_NullRetrieval()
        {
            var fromEnvironmentProvider = new FromEnvironmentProvider();
            Environment.SetEnvironmentVariable("ExistingKey", "ExistingKeyValue");

            var nullKeyValue = fromEnvironmentProvider.GetRawSetting(null);
        }
    }
}
