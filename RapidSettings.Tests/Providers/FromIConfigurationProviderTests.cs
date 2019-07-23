#if NETCOREAPP2_0 || NET47
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RapidSettings.Core;

namespace RapidSettings.Tests.Providers
{
    [TestClass]
    public class FromIConfigurationProviderTests
    {
        [TestMethod]
        public void FromIConfigurationProviderTests_SimpleRetrieval()
        {
            var iconfigurationMock = new Mock<IConfiguration>();
            iconfigurationMock.SetupGet(x => x["ExistingKey"]).Returns("ExistingKeyValue");
            var fromAppSettingsProvider = new FromIConfigurationProvider(iconfigurationMock.Object);

            var existingKeyValue = fromAppSettingsProvider.GetRawSetting("ExistingKey");

            Assert.AreEqual("ExistingKeyValue", existingKeyValue);
        }

        [TestMethod]
        public void FromIConfigurationProviderTests_NonExistingRetrieval()
        {
            var iconfigurationMock = new Mock<IConfiguration>();
            var fromAppSettingsProvider = new FromIConfigurationProvider(iconfigurationMock.Object);

            var notExistingKeyValue = fromAppSettingsProvider.GetRawSetting("NotExistingKey");

            Assert.IsNull(notExistingKeyValue, "Not existing key value is not null!");
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException), AllowDerivedTypes = true)]
        public void FromIConfigurationProviderTests_NullRetrieval()
        {
            var iconfigurationMock = new Mock<IConfiguration>();
            var fromAppSettingsProvider = new FromIConfigurationProvider(iconfigurationMock.Object);

            fromAppSettingsProvider.GetRawSetting(null);
        }
    }
}
#endif