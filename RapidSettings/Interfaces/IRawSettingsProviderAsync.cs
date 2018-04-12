using System.Threading.Tasks;

namespace RapidSettings.Core
{
    /// <summary>
    /// Gets raw values of settings with awaitable <see cref="Task"/>.
    /// </summary>
    public interface IRawSettingsProviderAsync : IRawSettingsProvider
    {
        /// <summary>
        /// Gets raw value of setting asynchronously.
        /// </summary>
        /// <param name="key">Key by which setting should be retrieved.</param>
        Task<object> GetRawSettingAsync(string key);
    }
}
