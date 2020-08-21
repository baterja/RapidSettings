﻿using System;
using System.Globalization;
using System.IO;

namespace RapidSettings.Core
{
    /// <summary>
    /// Provides converting methods from string to basic framework types (int, bool, etc.).
    /// </summary>
    public class StringToFrameworkTypesConverter : RawSettingsConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringToFrameworkTypesConverter"/> class.
        /// </summary>
        public StringToFrameworkTypesConverter()
        {
            this.AddSupportForTypes(typeof(string), typeof(Guid), (rawValue, type) => System.Convert.ChangeType(new Guid((string)rawValue), typeof(Guid)));
            this.AddSupportForTypes(typeof(string), typeof(bool), (rawValue, type) => bool.Parse((string)rawValue));
            this.AddSupportForTypes(typeof(string), typeof(byte), (rawValue, type) => byte.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(sbyte), (rawValue, type) => sbyte.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(char), (rawValue, type) => char.Parse((string)rawValue));
            this.AddSupportForTypes(typeof(string), typeof(decimal), (rawValue, type) => decimal.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(double), (rawValue, type) => double.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(float), (rawValue, type) => float.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(int), (rawValue, type) => int.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(uint), (rawValue, type) => uint.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(long), (rawValue, type) => long.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(ulong), (rawValue, type) => ulong.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(short), (rawValue, type) => short.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(ushort), (rawValue, type) => ushort.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(TimeSpan), (rawValue, type) => TimeSpan.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(DateTime), (rawValue, type) => DateTime.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(DateTimeOffset), (rawValue, type) => DateTimeOffset.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(Uri), (rawValue, type) => new Uri((string)rawValue));
            this.AddSupportForTypes(typeof(string), typeof(DirectoryInfo), (rawValue, type) => new DirectoryInfo((string)rawValue));
            this.AddSupportForTypes(typeof(string), typeof(FileInfo), (rawValue, type) => new FileInfo((string)rawValue));
        }
    }
}
