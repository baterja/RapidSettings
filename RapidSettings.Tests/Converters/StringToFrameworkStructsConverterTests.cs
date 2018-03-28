using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Converters;
using RapidSettings.Core;
using RapidSettings.Interfaces;
using RapidSettings.Providers;
using System;
using System.Linq;

namespace RapidSettings.Tests.Converters
{
    [TestClass]
    public class StringToFrameworkStructsConverterTests
    {
        private class NumericFrameworkStructsSettings
        {
            #region Fields
#pragma warning disable 649 // value never assigned ...
            [ToFill(nameof(int1))]
            public int int1;

            [ToFill(nameof(byte1))]
            public byte byte1;

            [ToFill(nameof(sbyte1))]
            public sbyte sbyte1;

            [ToFill(nameof(char1))]
            public char char1;

            [ToFill(nameof(decimal1))]
            public decimal decimal1;

            [ToFill(nameof(double1))]
            public double double1;

            [ToFill(nameof(float1))]
            public float float1;

            [ToFill(nameof(uint1))]
            public uint uint1;

            [ToFill(nameof(ulong1))]
            public ulong ulong1;

            [ToFill(nameof(short1))]
            public short short1;

            [ToFill(nameof(ushort1))]
            public ushort ushort1;
#pragma warning restore 649
            #endregion

            #region Properties
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
            #endregion
        }

        [TestMethod]
        public void NumericSimpleFrameworkStructsTest()
        {
            var settingsFiller = this.GetSettingsFiller();

            var settings = settingsFiller.CreateWithSettings<NumericFrameworkStructsSettings>();

            #region Fields
            Assert.AreEqual(1, settings.byte1);
            Assert.AreEqual('1', settings.char1);
            Assert.AreEqual(1, settings.decimal1);
            Assert.AreEqual(1, settings.double1);
            Assert.AreEqual(1, settings.float1);
            Assert.AreEqual(1, settings.int1);
            Assert.AreEqual(1, settings.sbyte1);
            Assert.AreEqual(1, settings.short1);
            Assert.AreEqual((uint)1, settings.uint1);
            Assert.AreEqual((ulong)1, settings.ulong1);
            Assert.AreEqual((ushort)1, settings.ushort1);
            #endregion

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

        private class NonNumericFrameworkStructsSettings
        {
            [ToFill]
            public bool SomeBool { get; private set; }

            [ToFill]
            public Guid SomeGuid { get; private set; }

            [ToFill]
            public string SomeString { get; private set; }
        }

        [TestMethod]
        public void NonNumericSimpleFrameworkStructsTest()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkStructsConverter() });
            var rawSettingsProvider = new FromFuncProvider(key =>
                key.ToString() == nameof(NonNumericFrameworkStructsSettings.SomeGuid)
                ? Guid.NewGuid().ToString()
                : key.ToString() == nameof(NonNumericFrameworkStructsSettings.SomeBool)
                    ? bool.TrueString
                    : "asdf"
            );
            var settingsFiller = new SettingsFiller(converterChooser, rawSettingsProvider);

            var settings = settingsFiller.CreateWithSettings<NonNumericFrameworkStructsSettings>();

            Assert.AreEqual(true, settings.SomeBool);
            Assert.IsTrue(default(Guid) != settings.SomeGuid);
            Assert.AreEqual("asdf", settings.SomeString);
        }

        private SettingsFiller GetSettingsFiller()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkStructsConverter() });
            var rawSettingsProvider = new FromFuncProvider(key => key.ToString().Last().ToString());
            var settingsFiller = new SettingsFiller(converterChooser, rawSettingsProvider);

            return settingsFiller;
        }
    }
}
