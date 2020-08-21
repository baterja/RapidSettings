using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RapidSettings.Core;

namespace RapidSettings.Tests.Converters
{
    [TestClass]
    public class NullableConverterTests
    {
        [TestMethod]
        public void CanConvertWhateverTypeToNullableType()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new NullableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(object), typeof(int?));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CanConvertNullableTypeToWhateverType()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new NullableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(double?), typeof(object));

            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CannotConvertNotNullableTypeToNotNullableType()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new NullableConverter(converterChooserMock.Object);

            var canConvert = converter.CanConvert(typeof(object), typeof(int));

            Assert.IsFalse(canConvert);
        }

        [TestMethod]
        public void ShouldCallConverterChooserWithNullForNullRawValue()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new NullableConverter(converterChooserMock.Object);
            var rawValue = new int?();

            converter.Convert<int?, double?>(rawValue);

            converterChooserMock.Verify(x => x.ChooseAndConvert<int?, double>(null));
        }

        [TestMethod]
        public void ShouldCallConverterChooserWithExtractedStructForNotNullRawValue()
        {
            var converterChooserMock = new Mock<ISettingsConverterChooser>();
            var converter = new NullableConverter(converterChooserMock.Object);
            var rawValue = new double?(5);

            converter.Convert<double?, int?>(rawValue);

            converterChooserMock.Verify(x => x.ChooseAndConvert<double, int>(5));
        }
    }
}
