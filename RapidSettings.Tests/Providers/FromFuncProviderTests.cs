using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;

namespace RapidSettings.Tests.Providers
{
    [TestClass]
    public class FromFuncProviderTests
    {
        [TestMethod]
        public void SimpleRetrieval()
        {
            var fromFuncProvider = new FromFuncProvider(key => key == "ExistingKey" ? "ExistingKeyValue" : null);

            var existingKeyValue = fromFuncProvider.GetRawSetting("ExistingKey");

            Assert.AreEqual("ExistingKeyValue", existingKeyValue);
        }

        [TestMethod]
        public void NonExistingRetrieval()
        {
            var fromFuncProvider = new FromFuncProvider(key => key == "ExistingKey" ? "ExistingKeyValue" : null);

            var notExistingKeyValue = fromFuncProvider.GetRawSetting("NotExistingKey");

            Assert.IsNull(notExistingKeyValue, "Not existing key value is not null!");
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException), AllowDerivedTypes = true)]
        public void NullRetrieval()
        {
            var fromFuncProvider = new FromFuncProvider(key => key == "ExistingKey" ? "ExistingKeyValue" : null);

            var nullKeyValue = fromFuncProvider.GetRawSetting(null);
        }
    }
}
