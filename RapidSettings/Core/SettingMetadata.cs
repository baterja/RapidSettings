using RapidSettings.Exceptions;
using RapidSettings.Interfaces;

namespace RapidSettings.Core
{
    /// <summary>
    /// Provides basic metadata about retrieval/conversion of setting.
    /// </summary>
    public class SettingMetadata : ISettingMetadata
    {
        /// <param name="key">Key by which this setting was retrieved.</param>
        /// <param name="isRequired">Indicates if successful retrieval and conversion was required.</param>
        /// <param name="hasValueSpecified">Indicates if value of setting was successfully retrieved and converted
        /// or if a default value was assigned for non-required setting which value couldn't be retrieved/converted.</param>
        public SettingMetadata(string key, bool isRequired, bool hasValueSpecified)
        {
            this.Key = key ?? throw new RapidSettingsException($"{nameof(key)} cannot be null!");
            this.IsRequired = isRequired;
            this.HasValueSpecified = hasValueSpecified;
        }

        /// <summary>
        /// Indicates if value of this setting was successfully retrieved and converted
        /// or if a default value was assigned for non-required setting which value couldn't be retrieved/converted.
        /// </summary>
        public bool HasValueSpecified { get; }

        /// <summary>
        /// Indicates if successful retrieval and conversion of this value was required.
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// Key which was used to retrieve this setting.
        /// </summary>
        public string Key { get; }
    }
}
