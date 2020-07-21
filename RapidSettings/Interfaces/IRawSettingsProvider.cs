namespace RapidSettings.Core
{
    /// <summary>
    /// Gets raw values of settings.
    /// </summary>
    public interface IRawSettingsProvider
    {
        /// <summary>
        /// Gets raw value of setting.
        /// </summary>
        /// <param name="key">Key by which setting should be retrieved.</param>
        object GetRawSetting(string key);
    }
}
