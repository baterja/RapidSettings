using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RapidSettings.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RapidSettings.Tests.Converters
{
    [TestClass]
    public class EnumerableConverterTests
    {
        [TestMethod]
        public void CanConvertIEnumerableOfWhateverToListOfWhatever()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(IEnumerable<int>), typeof(List<string>));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CanConvertIEnumerableOfWhateverToEnumerableOfWhatever()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(IEnumerable<int>), typeof(IEnumerable<string>));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CanConvertIEnumerableOfWhateverToInterfaceCompatibleWithListOfWhatever()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(IEnumerable<int>), typeof(IReadOnlyCollection<string>));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CanConvertIEnumerableOfWhateverToInterfaceCompatibleWithSetOfWhatever()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(IEnumerable<int>), typeof(ISet<string>));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CanConvertIEnumerableOfWhateverToHashSetOfWhatever()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(IEnumerable<int>), typeof(HashSet<string>));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CanConvertIEnumerableOfWhateverToDictionaryOfWhatever()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(IEnumerable<int>), typeof(Dictionary<string, double>));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CanConvertIEnumerableOfWhateverToInterfaceCompatibleWithDictionaryOfWhatever()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(IEnumerable<int>), typeof(IReadOnlyDictionary<string, double>));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CannotConvertIEnumerableOfWhateverToConcurrentBagOfKvps()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(IEnumerable<int>), typeof(ConcurrentBag<KeyValuePair<string, int>>));

            Assert.IsFalse(canConvert);
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException))]
        public void CannotConvertToNonGenericIEnumerableTypes()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            converterChooserMock.Setup(x => x.ChooseAndConvert<string, int>(It.IsAny<string>())).Returns<string>(str => int.Parse(str, CultureInfo.InvariantCulture));
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var rawValue = new string[] { "1", "2" };

            converter.Convert<string[], System.Collections.ICollection>(rawValue);
        }

        [TestMethod]
        public void ShouldCallConverterChooserWithValuesAndReturnCreatedList()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            converterChooserMock.Setup(x => x.ChooseAndConvert<string, int>(It.IsAny<string>())).Returns<string>(str => int.Parse(str, CultureInfo.InvariantCulture));
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var expectedList = new List<int> { 1, 2 };
            var rawValue = new string[] { "1", "2" };

            var convertedList = converter.Convert<string[], ICollection<int>>(rawValue);

            converterChooserMock.Verify(x => x.ChooseAndConvert<string, int>(rawValue[0]), Times.Once);
            converterChooserMock.Verify(x => x.ChooseAndConvert<string, int>(rawValue[1]), Times.Once);
            CollectionAssert.AreEqual(expectedList, (System.Collections.ICollection)convertedList);
        }

        [TestMethod]
        public void ShouldCallConverterChooserWithValuesAndReturnCreatedSet()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            converterChooserMock.Setup(x => x.ChooseAndConvert<string, int>(It.IsAny<string>())).Returns<string>(str => int.Parse(str, CultureInfo.InvariantCulture));
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var expectedList = new HashSet<int> { 1, 2 };
            var rawValue = new ConcurrentBag<string> { "1", "2", "2" };

            var convertedSet = converter.Convert<ConcurrentBag<string>, ISet<int>>(rawValue);

            converterChooserMock.Verify(x => x.ChooseAndConvert<string, int>(It.IsAny<string>()), Times.Exactly(3));
            CollectionAssert.AllItemsAreUnique(convertedSet.ToList());
            CollectionAssert.AreEquivalent(expectedList.ToList(), convertedSet.ToList());
        }

        [TestMethod]
        public void ShouldCallConverterChooserWithValuesAndReturnCreatedDict()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            converterChooserMock
                .Setup(x => x.ChooseAndConvert<KeyValuePair<string, string>, KeyValuePair<long, int>>(It.IsAny<KeyValuePair<string, string>>()))
                .Returns<KeyValuePair<string, string>>(kvp => new KeyValuePair<long, int>(long.Parse(kvp.Key, CultureInfo.InvariantCulture), int.Parse(kvp.Value, CultureInfo.InvariantCulture)));
            var converter = new EnumerableConverter(converterChooserMock.Object);

            var expectedDict = new Dictionary<long, int> { { 1, 1 }, { 2, 2 } };
            var rawValue = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("1", "1"),
                new KeyValuePair<string, string>("2", "2"),
            };

            var convertedDict = converter.Convert<KeyValuePair<string, string>[], IReadOnlyDictionary<long, int>>(rawValue);

            converterChooserMock.Verify(x => x.ChooseAndConvert<KeyValuePair<string, string>, KeyValuePair<long, int>>(It.IsAny<KeyValuePair<string, string>>()), Times.Exactly(2));
            CollectionAssert.AreEqual(new KeyValuePair<long, int>[0], expectedDict.Except(convertedDict).ToList());
        }
    }
}
