namespace RapidSettings.Core
{
    /// <summary>
    /// Wrapper for some setting value which contains <see cref="Metadata"/> about its retrieval and conversion.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="Value"/> which should be wrapped.</typeparam>
    public class Setting<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Setting{T}"/> class.
        /// </summary>
        /// <param name="metadata">Data about retrieval and conversion of this setting.</param>
        /// <param name="value">Setting value.</param>
        public Setting(T value, SettingMetadata metadata)
        {
            this.Metadata = metadata ?? throw new RapidSettingsException($"{nameof(metadata)} cannot be null! (Because otherwise this wrapper is useless)");
            this.Value = value;
        }

        /// <summary>
        /// Data about retrieval and conversion of this setting.
        /// </summary>
        public SettingMetadata Metadata { get; }

        /// <summary>
        /// Value of the setting.
        /// </summary>
        public T Value { get; }
    }
}
