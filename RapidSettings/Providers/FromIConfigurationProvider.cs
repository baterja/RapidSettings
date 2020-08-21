#if NETSTANDARD2_0 || NET47
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

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
        /// <remarks>
        /// If a section retrieved by given key has child sections instead of direct value, Dictionary&lt;string, object&gt; is created.
        /// If keys of created Dictionary&lt;string, object&gt; are consecutive integers beginning at 0, dictionary is converted to a List&lt;object&gt;.
        /// </remarks>
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

            var section = this.Configuration.GetSection(key);
            if (section == null)
            {
                return null;
            }

            var retrievedValue = GetValue(section);

            return retrievedValue;
        }

        private static object GetValue(IConfigurationSection rootSection)
        {
            if (TryGetDirectly(rootSection, out var directlyRetrievedValue))
            {
                return directlyRetrievedValue;
            }

            if (TryGetDictFromSection(rootSection, out var retrievedDict))
            {
                if (TryConvertDictToList(retrievedDict, out var convertedList))
                {
                    return convertedList;
                }

                return retrievedDict;
            }

            return null;
        }

        private static bool TryGetDirectly(IConfigurationSection rootSection, out string retrievedValue)
        {
            retrievedValue = rootSection.Value;
            return retrievedValue != null;
        }

        private static bool TryGetDictFromSection(IConfiguration rootSection, out IReadOnlyDictionary<string, object> retrievedDict)
        {
            retrievedDict = rootSection
                .GetChildren()
                .ToDictionary(subSection => subSection.Key, subSection => GetValue(subSection));

            return retrievedDict.Any();
        }

        private static bool TryConvertDictToList(IReadOnlyDictionary<string, object> dict, out IReadOnlyList<object> convertedList)
        {
            var listKeysSequence = Enumerable
                .Range(0, dict.Keys.Count())
                .Select(index => index.ToString());

            if (dict.Keys.SequenceEqual(listKeysSequence))
            {
                convertedList = dict.Values.ToList();
                return true;
            }

            convertedList = null;
            return false;
        }
    }
}
#endif