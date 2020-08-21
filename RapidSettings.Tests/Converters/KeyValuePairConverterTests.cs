using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RapidSettings.Core;
using System.Collections.Generic;

namespace RapidSettings.Tests.Converters
{
    [TestClass]
    public class KeyValuePairConverterTests
    {
        [TestMethod]
        public void CanConvertKvpToKvp()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new KeyValuePairConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(KeyValuePair<int, double>), typeof(KeyValuePair<string, object>));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CannotConvertKvpToNotKvp()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new KeyValuePairConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(KeyValuePair<int, double>), typeof(object));

            Assert.IsFalse(canConvert);
        }

        [TestMethod]
        public void CannotConvertNotKvpToKvp()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new KeyValuePairConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(object), typeof(KeyValuePair<string, object>));

            Assert.IsFalse(canConvert);
        }

        [TestMethod]
        public void ShouldCallConverterChooserWithKeyAndValueAndReturnCreatedKvp()
        {
            var obj = new object();
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            converterChooserMock.Setup(x => x.ChooseAndConvert<string, object>(It.IsAny<string>())).Returns(obj);
            converterChooserMock.Setup(x => x.ChooseAndConvert<int, double>(It.IsAny<int>())).Returns(-1.1);

            var exprectedKvp = new KeyValuePair<object, double>(obj, -1.1);
            var converter = new KeyValuePairConverter(converterChooserMock.Object);
            var rawValue = new KeyValuePair<string, int>("test", 1);

            var convertedKvp = converter.Convert<KeyValuePair<string, int>, KeyValuePair<object, double>>(rawValue);

            converterChooserMock.Verify(x => x.ChooseAndConvert<string, object>(rawValue.Key), Times.Once);
            converterChooserMock.Verify(x => x.ChooseAndConvert<int, double>(rawValue.Value), Times.Once);
            Assert.AreEqual(exprectedKvp, convertedKvp);
        }
    }
}
