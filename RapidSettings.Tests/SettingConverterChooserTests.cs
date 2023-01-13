using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSettings.Core;
using System;

namespace RapidSettings.Tests.Attributes
{
    [TestClass]
    public class SettingConverterChooserTests
    {
        [TestMethod]
        public void StringToItsInterface()
        {
            var settingConverterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });

            var strVal = settingConverterChooser.ChooseAndConvert<string, IConvertible>("123");

            Assert.AreEqual("123", strVal);
        }

        private class A { }

        private class B : A { }

        private class C : B { }

        private class SuperConverter : RawSettingsConverterBase
        {
            public SuperConverter()
            {
                this.AddSupportForTypes(typeof(A), typeof(A), (rawValue, type) => rawValue);
                this.AddSupportForTypes(typeof(C), typeof(C), (rawValue, type) => rawValue);
            }
        }

        [TestMethod]
        public void TFromCovariance()
        {
            var settingConverterChooser = new SettingsConverterChooser(new[] { new SuperConverter() });

            var bInstance = new B();
            var aInstance = settingConverterChooser.ChooseAndConvert<B, A>(bInstance);

            Assert.AreSame(bInstance, aInstance);
        }

        [TestMethod]
        public void TToCovariance()
        {
            var settingConverterChooser = new SettingsConverterChooser(new[] { new SuperConverter() });

            var cInstance = new C();
            var bInstance = settingConverterChooser.ChooseAndConvert<C, B>(cInstance);

            Assert.AreSame(cInstance, bInstance);
        }

        [TestMethod]
        [ExpectedException(typeof(RapidSettingsException))]
        public void NoSuitableConverter()
        {
            var settingConverterChooser = new SettingsConverterChooser(new[] { new SuperConverter() });

            settingConverterChooser.ChooseAndConvert<int, string>(3);
        }

        [TestMethod]
        public void ChooseProperConverter()
        {
            var settingConverterChooser = new SettingsConverterChooser(new IRawSettingsConverter[] { new SuperConverter(), new StringToFrameworkTypesConverter() });

            var bInstance = new B();
            var aInstance = settingConverterChooser.ChooseAndConvert<B, A>(bInstance);
            var strVal = settingConverterChooser.ChooseAndConvert<string, IConvertible>("123");

            Assert.AreSame(bInstance, aInstance);
            Assert.AreEqual("123", strVal);
        }
    }
}
