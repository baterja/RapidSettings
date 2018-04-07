using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Exceptions;
using RapidSettings.Providers;

namespace RapidSettings.Tests.Converters
{
    [TestClass]
    public class FromFuncProviderTests
    {
        [TestMethod]
        public void FromFuncProviderTest_SimpleRetrieval()
        {
            var fromFuncProvider = new FromFuncProvider(key => key == "ExistingKey" ? "ExistingKeyValue" : null);

            var existingKeyValue = fromFuncProvider.GetRawSetting("ExistingKey");

            Assert.AreEqual("ExistingKeyValue", existingKeyValue);
        }

        [TestMethod]
        public void FromFuncProviderTest_NonExistingRetrieval()
        {
            var fromFuncProvider = new FromFuncProvider(key => key == "ExistingKey" ? "ExistingKeyValue" : null);

            var notExistingKeyValue = fromFuncProvider.GetRawSetting("NotExistingKey");

            Assert.IsNull(notExistingKeyValue, "Not existing key value is not null!");
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException), AllowDerivedTypes = true)]
        public void FromFuncProviderTest_NullRetrieval()
        {
            var fromFuncProvider = new FromFuncProvider(key => key == "ExistingKey" ? "ExistingKeyValue" : null);

            var nullKeyValue = fromFuncProvider.GetRawSetting(null);
        }
    }
}
