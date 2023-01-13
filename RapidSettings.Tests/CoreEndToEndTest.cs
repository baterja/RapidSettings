using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace RapidSettings.Tests
{
    [TestClass]
    public class CoreEndToEndTest
    {
        private class SomeSettings
        {
            [ToFill]
            public string TestString { get; private set; }

            [ToFill]
            public int TestInt { get; private set; }

            [ToFill]
            public double TestDouble { get; private set; }

            [ToFill]
            public double? TestNullableDouble { get; private set; }

            [ToFill]
            public ICollection<long> TestList { get; private set; }

            [ToFill]
            public IReadOnlyDictionary<string, int> TestDict { get; private set; }

            [ToFill]
            public Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>> TestComplexDict { get; private set; }
        }

        [TestMethod]
        public void EndToEndTest()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var settingsFiller = new SettingsFiller(defaultRawSettingsProvider: new FromIConfigurationProvider(configuration));

            var settings = settingsFiller.CreateWithSettings<SomeSettings>();

            Assert.AreEqual("String", settings.TestString);
            Assert.AreEqual(5, settings.TestInt);
            Assert.AreEqual(4.5, settings.TestDouble);
            Assert.AreEqual(1.2, settings.TestNullableDouble);

            var expectedList = new List<long> { 1, 2, 3 };
            CollectionAssert.AreEqual(expectedList, settings.TestList.ToList());

            var expectedKvps = new KeyValuePair<string, int>[]
            {
                new KeyValuePair<string, int>("A", 1),
                new KeyValuePair<string, int>("B", 2),
            };
            Assert.IsTrue(!settings.TestDict.Except(expectedKvps).Any());

            var expectedDict = new Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>>
            {
                {
                    "A", new Dictionary<string, Dictionary<string, List<int>>>
                {
                    {
                        "B", new Dictionary<string, List<int>> { { "C", new List<int> { 1, 2, 3 } } }
                    },
                    {
                        "B1", new Dictionary<string, List<int>> { { "C1", new List<int> { 4, 5, 6 } } }
                    },
                }
                },
            };

            var expectedDictJson = JsonSerializer.Serialize(expectedDict);
            var actualDictJson = JsonSerializer.Serialize(settings.TestComplexDict);

            Assert.AreEqual(expectedDictJson, actualDictJson);
        }
    }
}
