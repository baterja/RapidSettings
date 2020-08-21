using System;
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
            this.AddSupportForTypes(typeof(string), typeof(Guid), (rawValue, _) => System.Convert.ChangeType(new Guid((string)rawValue), typeof(Guid), CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(bool), (rawValue, _) => bool.Parse((string)rawValue));
            this.AddSupportForTypes(typeof(string), typeof(byte), (rawValue, _) => byte.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(sbyte), (rawValue, _) => sbyte.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(char), (rawValue, _) => char.Parse((string)rawValue));
            this.AddSupportForTypes(typeof(string), typeof(decimal), (rawValue, _) => decimal.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(double), (rawValue, _) => double.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(float), (rawValue, _) => float.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(int), (rawValue, _) => int.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(uint), (rawValue, _) => uint.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(long), (rawValue, _) => long.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(ulong), (rawValue, _) => ulong.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(short), (rawValue, _) => short.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(ushort), (rawValue, _) => ushort.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(TimeSpan), (rawValue, _) => TimeSpan.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(DateTime), (rawValue, _) => DateTime.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(DateTimeOffset), (rawValue, _) => DateTimeOffset.Parse((string)rawValue, CultureInfo.InvariantCulture));
            this.AddSupportForTypes(typeof(string), typeof(Uri), (rawValue, _) => new Uri((string)rawValue));
            this.AddSupportForTypes(typeof(string), typeof(DirectoryInfo), (rawValue, _) => new DirectoryInfo((string)rawValue));
            this.AddSupportForTypes(typeof(string), typeof(FileInfo), (rawValue, _) => new FileInfo((string)rawValue));
        }
    }
}
