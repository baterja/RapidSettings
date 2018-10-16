using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;
using System;
using System.IO;
using System.Linq;

namespace RapidSettings.Tests.Converters
{
    [TestClass]
    public class StringToFrameworkTypesConverterTests
    {
        private class NumericFrameworkTypesSettings
        {
            [ToFill]
            public int Int1 { get; private set; }

            [ToFill]
            public byte Byte1 { get; private set; }

            [ToFill]
            public sbyte Sbyte1 { get; private set; }

            [ToFill]
            public char Char1 { get; private set; }

            [ToFill]
            public decimal Decimal1 { get; private set; }

            [ToFill]
            public double Double1 { get; private set; }

            [ToFill]
            public float Float1 { get; private set; }

            [ToFill]
            public uint Uint1 { get; private set; }

            [ToFill]
            public ulong Ulong1 { get; private set; }

            [ToFill]
            public short Short1 { get; private set; }

            [ToFill]
            public ushort Ushort1 { get; private set; }
        }

        [TestMethod]
        public void NumericSimpleFrameworkStructsTest()
        {
            var settingsFiller = this.GetSettingsFiller();

            var settings = settingsFiller.CreateWithSettings<NumericFrameworkTypesSettings>();

            #region Properties
            Assert.AreEqual(1, settings.Byte1);
            Assert.AreEqual('1', settings.Char1);
            Assert.AreEqual(1, settings.Decimal1);
            Assert.AreEqual(1, settings.Double1);
            Assert.AreEqual(1, settings.Float1);
            Assert.AreEqual(1, settings.Int1);
            Assert.AreEqual(1, settings.Sbyte1);
            Assert.AreEqual(1, settings.Short1);
            Assert.AreEqual((uint)1, settings.Uint1);
            Assert.AreEqual((ulong)1, settings.Ulong1);
            Assert.AreEqual((ushort)1, settings.Ushort1);
            #endregion
        }

        private class NullableFrameworkStructsSettings
        {
            [ToFill]
            public int? Int1 { get; private set; }
        }

        [TestMethod]
        public void NullableFrameworkStructsSettingsTest()
        {
            var settingsFiller = this.GetSettingsFiller();

            var settings = settingsFiller.CreateWithSettings<NullableFrameworkStructsSettings>();

            Assert.AreEqual(1, settings.Int1.Value);
        }

        private class NonNumericFrameworkTypesSettings
        {
            [ToFill]
            public bool SomeBoolean { get; private set; }

            [ToFill]
            public Guid SomeGuid { get; private set; }

            [ToFill]
            public string SomeString { get; private set; }

            [ToFill]
            public DateTime SomeDateTime { get; private set; }

            [ToFill]
            public DateTimeOffset SomeDateTimeOffset { get; private set; }

            [ToFill]
            public Uri SomeUri { get; private set; }

            [ToFill]
            public DirectoryInfo SomeDirectoryInfo { get; private set; }

            [ToFill]
            public FileInfo SomeFileInfo { get; private set; }
        }

        [TestMethod]
        public void NonNumericSimpleFrameworkStructsTest()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });
            var rawSettingsProvider = new FromFuncProvider(key =>
            {
                switch (key)
                {
                    case string x when x.EndsWith(typeof(Guid).Name, StringComparison.InvariantCultureIgnoreCase):
                        return Guid.NewGuid().ToString();
                    case string x when x.EndsWith(typeof(bool).Name, StringComparison.InvariantCultureIgnoreCase):
                        return bool.TrueString;
                    case string x when x.EndsWith(typeof(string).Name, StringComparison.InvariantCultureIgnoreCase):
                        return "asdf";
                    case string x when x.EndsWith(typeof(DateTime).Name, StringComparison.InvariantCultureIgnoreCase):
                        return "2000-01-01";
                    case string x when x.EndsWith(typeof(DateTimeOffset).Name, StringComparison.InvariantCultureIgnoreCase):
                        return "2000-01-01";
                    case string x when x.EndsWith(typeof(Uri).Name, StringComparison.InvariantCultureIgnoreCase):
                        return "https://nuget.org";
                    case string x when x.EndsWith(typeof(DirectoryInfo).Name, StringComparison.InvariantCultureIgnoreCase):
                        return "D:\\SomeFolder";
                    case string x when x.EndsWith(typeof(FileInfo).Name, StringComparison.InvariantCultureIgnoreCase):
                        return "filename.txt";
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(key)} with value {key} is out of range of handled keys!");
                }
            });

            var settingsFiller = new SettingsFiller(converterChooser, rawSettingsProvider);

            var settings = settingsFiller.CreateWithSettings<NonNumericFrameworkTypesSettings>();

            Assert.AreEqual(true, settings.SomeBoolean);
            Assert.IsTrue(default(Guid) != settings.SomeGuid);
            Assert.AreEqual("asdf", settings.SomeString);
            Assert.AreEqual(DateTime.Parse("2000-01-01"), settings.SomeDateTime);
            Assert.AreEqual(DateTimeOffset.Parse("2000-01-01"), settings.SomeDateTimeOffset);
            Assert.AreEqual(new Uri("https://nuget.org"), settings.SomeUri);
            Assert.AreEqual(new DirectoryInfo("D:\\SomeFolder").FullName, settings.SomeDirectoryInfo.FullName);
            Assert.AreEqual(new FileInfo("filename.txt").FullName, settings.SomeFileInfo.FullName);
        }

        private SettingsFiller GetSettingsFiller()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });
            var rawSettingsProvider = new FromFuncProvider(key => key.ToString().Last().ToString());
            var settingsFiller = new SettingsFiller(converterChooser, rawSettingsProvider);

            return settingsFiller;
        }
    }
}
