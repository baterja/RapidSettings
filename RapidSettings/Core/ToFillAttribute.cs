using System;
using System.Runtime.CompilerServices;
using RapidSettings.Exceptions;
using RapidSettings.Interfaces;

namespace RapidSettings.Core
{
    /// <summary>
    /// Indicates that marked field or property should be filled by <see cref="ISettingsFiller"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ToFillAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToFillAttribute"/> class.
        /// </summary>
        /// <param name="key">Key that will be used to retrieve setting's value. <see cref="CallerMemberNameAttribute"/> will be used as default (works only with properties!).</param>
        /// <param name="isRequired">Indicates if decorated member is required. True by default.</param>
        /// <param name="rawSettingsProviderName">Provider that will receive <paramref name="key"/> to provide raw value of decorated setting that will be later converted to proper type.
        /// Leaving null there causes used <see cref="ISettingsFiller"/> to use its default provider.</param>
        public ToFillAttribute([CallerMemberName] string key = null, bool isRequired = true, string rawSettingsProviderName = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new RapidSettingsException($"{nameof(key)} cannot be null or empty!");
            }

            this.IsRequired = isRequired;
            this.Key = key;
            this.RawSettingsProviderName = rawSettingsProviderName;
        }

        /// <summary>
        /// Indicates if decorated member is required. True by default.
        /// </summary>
        /// <remarks>
        /// If resolution or conversion of required member fails, exception will be raised. Default value of expected type will be assigned for non-required member.
        /// </remarks>
        public bool IsRequired { get; }

        /// <summary>
        /// Key that will be used to retrieve raw setting value.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Name of provider that will receive <see cref="Key"/> to provide raw value of decorated setting that will be later converted to proper type.
        /// Leaving null there cause that used <see cref="ISettingsFiller"/> will use its default provider.
        /// </summary>
        public string RawSettingsProviderName { get; }
    }
}
