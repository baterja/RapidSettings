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
        public object GetRawSetting(object key)
        {
            if (key == null)
            {
                throw new RapidSettingsException($"{nameof(key)} cannot be null!");
            }

            if (!(key is string keyString))
            {
                throw new RapidSettingsException($"{nameof(key)} is not a string! Its type is: {key.GetType().Name}");
            }

            if (keyString == string.Empty)
            {
                throw new RapidSettingsException($"{nameof(key)} cannot be empty!");
            }

            return ConfigurationManager.AppSettings.Get(keyString);
        }
    }
}
