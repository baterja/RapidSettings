using System;
using System.Collections.Generic;

namespace RapidSettings.Core
{
    /// <summary>
    /// Indicates that marked class should be filled by <see cref="ISettingsFiller"/>.
    /// Behavior is identical to marking every publicly settable property with <see cref="ToFillAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ClassToFillAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassToFillAttribute"/> class.
        /// </summary>
        /// <param name="keyPrefix">
        /// Prefix for all properties' <see cref="ToFillAttribute.Key"/>s.
        /// Class name + ":" will be used as the default.
        /// </param>
        /// <param name="allRequired">Indicates if decorated member is required. True by default.</param>
        /// <param name="rawSettingsProviderName">
        /// Provider that will receive <see cref="KeyPrefix"/> + <see cref="ToFillAttribute.Key"/>
        /// to provide raw value of decorated setting that will be later converted to proper type.
        /// Leaving null there causes used <see cref="ISettingsFiller"/> to use its default provider.</param>
        public ClassToFillAttribute(string keyPrefix = null, bool allRequired = true, string rawSettingsProviderName = null)
        {
            this.AllRequired = allRequired;
            this.KeyPrefix = keyPrefix;
            this.RawSettingsProviderName = rawSettingsProviderName;
        }

        /// <summary>
        /// Indicates if decorated member is required. True by default.
        /// </summary>
        /// <remarks>
        /// If resolution or conversion of required member fails, exception will be raised. Default value of expected type will be assigned for non-required member.
        /// For an <see cref="IEnumerable{T}"/> property, all its elements will also be required if the collection itself is required.
        /// </remarks>
        public bool AllRequired { get; }

        /// <summary>
        /// Prefix for all properties' <see cref="ToFillAttribute.Key"/>s. Class name + ":" will be used if null.
        /// </summary>
        public string KeyPrefix { get; }

        /// <summary>
        /// Name of provider that will receive <see cref="KeyPrefix"/> + <see cref="ToFillAttribute.Key"/> to provide raw value of decorated setting that will be later converted to proper type.
        /// Leaving null there cause that used <see cref="ISettingsFiller"/> will use its default provider.
        /// </summary>
        public string RawSettingsProviderName { get; }
    }
}
