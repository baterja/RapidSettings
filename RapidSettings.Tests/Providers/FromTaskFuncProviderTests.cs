using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;
using System.Threading.Tasks;

namespace RapidSettings.Tests.Providers
{
    [TestClass]
    public class FromTaskFuncProviderTests
    {
        [TestMethod]
        public async Task FromTaskFuncProviderTest_SimpleRetrieval()
        {
            var fromTaskFuncProvider = new FromTaskFuncProvider(key => Task.Factory.StartNew<object>(() => key == "ExistingKey" ? "ExistingKeyValue" : null));

            var existingKeyValue = await fromTaskFuncProvider.GetRawSettingAsync("ExistingKey");

            Assert.AreEqual("ExistingKeyValue", existingKeyValue);
        }

        [TestMethod]
        public async Task FromTaskFuncProviderTest_NonExistingRetrieval()
        {
            var fromTaskFuncProvider = new FromTaskFuncProvider(key => Task.Factory.StartNew<object>(() => key == "ExistingKey" ? "ExistingKeyValue" : null));

            var notExistingKeyValue = await fromTaskFuncProvider.GetRawSettingAsync("NotExistingKey");

            Assert.IsNull(notExistingKeyValue, "Not existing key value is not null!");
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException), AllowDerivedTypes = true)]
        public async Task FromTaskFuncProviderTest_NullRetrieval()
        {
            var fromTaskFuncProvider = new FromTaskFuncProvider(key => Task.Factory.StartNew<object>(() => key == "ExistingKey" ? "ExistingKeyValue" : null));

            var nullKeyValue = await fromTaskFuncProvider.GetRawSettingAsync(null);
        }
    }
}
