namespace RapidSettings.Interfaces
{
    /// <summary>
    /// Metadata that can be easily produced while retrieving and converting setting value.
    /// </summary>
    public interface ISettingMetadata
    {
        /// <summary>
        /// Indicates if value of this setting was successfully retrieved and converted
        /// or if a default value was assigned for non-required setting which value couldn't be retrieved/converted.
        /// </summary>
        bool HasValueSpecified { get; }

        /// <summary>
        /// Indicates if successful retrieval and conversion of this value was required.
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        /// Key which was used to retrieve this setting.
        /// </summary>
        object Key { get; }
    }
}
