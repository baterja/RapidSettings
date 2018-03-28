using RapidSettings.Exceptions;
using RapidSettings.Interfaces;
using System;

namespace RapidSettings.Providers
{
    /// <summary>
    /// Provides raw setting value from environment variable.
    /// </summary>
    public class FromEnvironmentProvider : IRawSettingsProviderSync
    {
        /// <summary>
        /// Gets raw setting value from environment variable named as <paramref name="key"/>.
        /// </summary>
        /// <param name="key">String name of environment variable.</param>
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

            return Environment.GetEnvironmentVariable(keyString);
        }
    }
}
