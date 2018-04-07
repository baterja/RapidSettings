using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Converters;
using RapidSettings.Core;
using RapidSettings.Exceptions;
using RapidSettings.Interfaces;
using System;

namespace RapidSettings.Tests.Attributes
{
    [TestClass]
    public class SettingConverterChooserTests
    {
        [TestMethod]
        public void SettingConverterChooserTest_StringToItsInterface()
        {
            var settingConverterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkStructsConverter() });

            IConvertible strVal = settingConverterChooser.ChooseAndConvert<string, IConvertible>("123");

            Assert.AreEqual("123", strVal);
        }

        private class A { }
        private class B : A { }
        private class C : B { }

        private class SuperConverter : RawSettingsConverterBase
        {
            public SuperConverter()
            {
                AddSupportForTypes(typeof(A), typeof(A), (rawValue, type) => rawValue);
                AddSupportForTypes(typeof(C), typeof(C), (rawValue, type) => rawValue);
            }
        }

        [TestMethod]
        public void SettingConverterChooserTest_TFromCovariance()
        {
            var settingConverterChooser = new SettingsConverterChooser(new[] { new SuperConverter() });

            var bInstance = new B();
            A aInstance = settingConverterChooser.ChooseAndConvert<B, A>(bInstance);

            Assert.AreSame(bInstance, aInstance);
        }

        [TestMethod]
        public void SettingConverterChooserTest_TToCovariance()
        {
            var settingConverterChooser = new SettingsConverterChooser(new[] { new SuperConverter() });

            var cInstance = new C();
            B bInstance = settingConverterChooser.ChooseAndConvert<C, B>(cInstance);

            Assert.AreSame(cInstance, bInstance);
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException))]
        public void SettingConverterChooserTest_NoSuitableConverter()
        {
            var settingConverterChooser = new SettingsConverterChooser(new[] { new SuperConverter() });

            settingConverterChooser.ChooseAndConvert<int, string>(3);
        }

        [TestMethod]
        public void SettingConverterChooserTest_ChooseProperConverter()
        {
            var settingConverterChooser = new SettingsConverterChooser(new IRawSettingsConverter[] { new SuperConverter(), new StringToFrameworkStructsConverter() });

            var bInstance = new B();
            A aInstance = settingConverterChooser.ChooseAndConvert<B, A>(bInstance);
            IConvertible strVal = settingConverterChooser.ChooseAndConvert<string, IConvertible>("123");

            Assert.AreSame(bInstance, aInstance);
            Assert.AreEqual("123", strVal);
        }
    }
}
