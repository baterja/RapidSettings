namespace RapidSettings.Interfaces
{
    /// <summary>
    /// Provides interface for setting with some retrieval/conversion metadata.
    /// </summary>
    public interface ISetting<T>
    {
        /// <summary>
        /// Data about retrieval and conversion of this setting.
        /// </summary>
        ISettingMetadata Metadata { get; }

        /// <summary>
        /// Value of the setting.
        /// </summary>
        T Value { get; }
    }
}
