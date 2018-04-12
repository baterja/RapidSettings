using System;

namespace RapidSettings.Core
{
    /// <summary>
    /// Customizable settings provider which provides its value by a func.
    /// </summary>
    public class FromFuncProvider : IRawSettingsProviderSync
    {
        private readonly Func<string, object> rawSettingResolvingFunc;

        /// <summary>
        /// Initializes a new instance of <see cref="FromFuncProvider"/> class.
        /// </summary>
        /// <param name="rawSettingResolvingFunc">Func which will be used to get setting's raw value.</param>
        public FromFuncProvider(Func<string, object> rawSettingResolvingFunc)
        {
            this.rawSettingResolvingFunc = rawSettingResolvingFunc ?? throw new RapidSettingsException($"{nameof(rawSettingResolvingFunc)} cannot be null!");
        }

        /// <summary>
        /// Gets raw setting value by provided <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key by which raw setting value should be retrieved.</param>
        /// <returns>Task which retrieves raw setting value.</returns>
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

            return this.rawSettingResolvingFunc.Invoke(key);
        }
    }
}
