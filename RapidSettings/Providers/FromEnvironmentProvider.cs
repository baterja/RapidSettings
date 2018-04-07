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

            return Environment.GetEnvironmentVariable(key);
        }
    }
}
