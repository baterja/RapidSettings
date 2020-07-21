#if NETSTANDARD2_0 || NET47
using Microsoft.Extensions.Configuration;

namespace RapidSettings.Core
{
    /// <summary>
    /// Provides raw setting value from given <see cref="IConfiguration"/>.
    /// </summary>
    public class FromIConfigurationProvider : IRawSettingsProvider
    {
        /// <summary>
        /// Source <see cref="IConfiguration"/> of this provider's instance.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="FromIConfigurationProvider"/> class.
        /// </summary>
        /// <param name="configuration"><see cref="IConfiguration"/> which will be used as a source of raw values by <see cref="GetRawSetting(string)"/>.</param>
        public FromIConfigurationProvider(IConfiguration configuration)
        {
            this.Configuration = configuration ?? throw new RapidSettingsException($"{nameof(configuration)} cannot be null or empty!"); ;
        }

        /// <summary>
        /// Gets raw setting value from <see cref="Configuration"/> by given string <paramref name="key"/>.
        /// </summary>
        /// <param name="key">String by which raw setting value should be retrieved from <see cref="Configuration"/></param>
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

            return this.Configuration[key];
        }
    }
}
#endif