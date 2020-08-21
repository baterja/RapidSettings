#if NETCOREAPP3_1 || NET47
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RapidSettings.Core;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RapidSettings.Tests.Providers
{
    [TestClass]
    public class FromIConfigurationProviderTests
    {
        [TestMethod]
        public void FromIConfigurationProviderTests_SimpleRetrieval()
        {
            var iconfigurationMock = new Mock<IConfiguration>();

            var simpleKeySectionMock = new Mock<IConfigurationSection>();
            simpleKeySectionMock.SetupGet(x => x.Value).Returns("ExistingKeyValue");
            iconfigurationMock.Setup(x => x.GetSection("ExistingKey")).Returns(simpleKeySectionMock.Object);

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
        public void FromIConfigurationProviderTests_ListRetrieval()
        {
            var iconfigurationMock = new Mock<IConfiguration>();

            var listSectionChildrenMocks = new List<Mock<IConfigurationSection>>
            {
                new Mock<IConfigurationSection>(),
                new Mock<IConfigurationSection>(),
                new Mock<IConfigurationSection>(),
            };

            for (var i = 0; i < listSectionChildrenMocks.Count; i++)
            {
                listSectionChildrenMocks[i].SetupGet(x => x.Key).Returns(i.ToString(CultureInfo.InvariantCulture));
                listSectionChildrenMocks[i].SetupGet(x => x.Value).Returns((i * 2).ToString(CultureInfo.InvariantCulture));
            }

            var listSectionChildren = listSectionChildrenMocks.Select(x => x.Object).ToList();
            var listSectionMock = new Mock<IConfigurationSection>();
            listSectionMock.Setup(x => x.GetChildren()).Returns(listSectionChildren);

            iconfigurationMock.Setup(x => x.GetSection("ListKey")).Returns(listSectionMock.Object);

            var fromAppSettingsProvider = new FromIConfigurationProvider(iconfigurationMock.Object);

            var expectedList = new List<string> { "0", "2", "4" };

            var retrievedList = fromAppSettingsProvider.GetRawSetting("ListKey");

            CollectionAssert.AreEqual(expectedList, (ICollection)retrievedList);
        }

        [TestMethod]
        public void FromIConfigurationProviderTests_DictionaryRetrieval()
        {
            var iconfigurationMock = new Mock<IConfiguration>();

            var dictSectionChildrenMocks = new List<Mock<IConfigurationSection>>
            {
                new Mock<IConfigurationSection>(),
                new Mock<IConfigurationSection>(),
                new Mock<IConfigurationSection>(),
            };

            var dictKeys = new List<string> { "a", "b", "c" };

            for (var i = 0; i < dictSectionChildrenMocks.Count; i++)
            {
                dictSectionChildrenMocks[i].SetupGet(x => x.Key).Returns(dictKeys[i]);
                dictSectionChildrenMocks[i].SetupGet(x => x.Value).Returns((i * 2).ToString(CultureInfo.InvariantCulture));
            }

            var dictSectionChildren = dictSectionChildrenMocks.Select(x => x.Object).ToList();
            var dictSectionMock = new Mock<IConfigurationSection>();
            dictSectionMock.Setup(x => x.GetChildren()).Returns(dictSectionChildren);

            iconfigurationMock.Setup(x => x.GetSection("DictKey")).Returns(dictSectionMock.Object);

            var fromAppSettingsProvider = new FromIConfigurationProvider(iconfigurationMock.Object);

            var expectedDict = new Dictionary<string, string> { { "a", "0" }, { "b", "2" }, { "c", "4" } };

            var retrievedDict = (Dictionary<string, object>)fromAppSettingsProvider.GetRawSetting("DictKey");

            CollectionAssert.AreEquivalent(expectedDict.Keys, retrievedDict.Keys);
            CollectionAssert.AreEquivalent(expectedDict.Values, retrievedDict.Values);
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