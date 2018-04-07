using RapidSettings.Exceptions;
using RapidSettings.Interfaces;
using System.Configuration;

namespace RapidSettings.Providers
{
    /// <summary>
    /// Provides raw setting value from app.config/web.config.
    /// </summary>
    public class FromAppSettingsProvider : IRawSettingsProviderSync
    {
        /// <summary>
        /// Gets raw setting value from app.config/web.config by given string <paramref name="key"/>.
        /// </summary>
        /// <param name="key">String by which raw setting value should be retrieved from app.config/web.config</param>
        public object GetRawSetting(string key)
        {
            if (key == null)
            {
                throw new RapidSettingsException($"{nameof(key)} cannot be null!");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new RapidSettingsException($"{nameof(key)} cannot be empty!");
            }

            return ConfigurationManager.AppSettings.Get(key);
        }
    }
}
