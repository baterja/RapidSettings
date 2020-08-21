using System.Collections.Generic;
using System.Linq;

namespace RapidSettings.Core
{
    /// <summary>
    /// Default values for <see cref="SettingsFiller"/> if <see cref="SettingsFiller(ISettingsConverterChooser, IDictionary{string, IRawSettingsProvider}, IRawSettingsProvider)"/> constructor is used with some nulls.
    /// </summary>
    public static class SettingsFillerStaticDefaults
    {
        static SettingsFillerStaticDefaults()
        {
            DefaultSettingsConverterChooser = new SettingsConverterChooser(Enumerable.Empty<IRawSettingsConverter>());
            DefaultRawSettingsConverters = new HashSet<IRawSettingsConverter> {
                new StringToFrameworkTypesConverter(),
                new EnumerableConverter(DefaultSettingsConverterChooser),
                new KeyValuePairConverter(DefaultSettingsConverterChooser),
                new NullableConverter(DefaultSettingsConverterChooser),
            };

            foreach (var rawSettingConverter in DefaultRawSettingsConverters)
            {
                DefaultSettingsConverterChooser.Converters.Add(rawSettingConverter);
            }
        }

        /// <summary>
        /// Key by which <see cref="FromAppSettingsProvider"/> is registered by default (if <see cref="SettingsFiller(ISettingsConverterChooser, IDictionary{string, IRawSettingsProvider}, IRawSettingsProvider)"/> constructor is used without specifying providers).
        /// </summary>
        public const string FromAppSettingsProviderKey = "AppSettings";

        /// <summary>
        /// Instance of <see cref="Core.FromAppSettingsProvider"/> which is passed by default to <see cref="SettingsFiller(ISettingsConverterChooser, IDictionary{string, IRawSettingsProvider}, IRawSettingsProvider)"/> constructor if it's used without specifying providers.
        /// </summary>
        public static FromAppSettingsProvider FromAppSettingsProvider { get; } = new FromAppSettingsProvider();

        /// <summary>
        /// Key by which <see cref="FromEnvironmentProvider"/> is registered by default (if <see cref="SettingsFiller(ISettingsConverterChooser, IDictionary{string, IRawSettingsProvider}, IRawSettingsProvider)"/> constructor is used without specifying providers).
        /// </summary>
        public const string FromEnvironmentProviderKey = "ENV";

        /// <summary>
        /// Instance of <see cref="Core.FromEnvironmentProvider"/> which is passed by default to <see cref="SettingsFiller(ISettingsConverterChooser, IDictionary{string, IRawSettingsProvider}, IRawSettingsProvider)"/> constructor if it's used without specifying providers.
        /// </summary>
        public static FromEnvironmentProvider FromEnvironmentProvider { get; } = new FromEnvironmentProvider();

        /// <summary>
        /// Default converters which are used to construct <see cref="DefaultSettingsConverterChooser"/>. By default, there are: 
        /// <see cref="StringToFrameworkTypesConverter"/>, <see cref="EnumerableConverter"/>, <see cref="KeyValuePairConverter"/> and <see cref="NullableConverter"/>.
        /// </summary>
        public static ISet<IRawSettingsConverter> DefaultRawSettingsConverters { get; }

        /// <summary>
        /// Instance of <see cref="SettingsConverterChooser"/> which is passed by default to <see cref="SettingsFiller(ISettingsConverterChooser, IDictionary{string, IRawSettingsProvider}, IRawSettingsProvider)"/> constructor if it's used without specifying converter chooser.
        /// </summary>
        public static ISettingsConverterChooser DefaultSettingsConverterChooser { get; set; }

        /// <summary>
        /// Settings provider which is passed by default to <see cref="SettingsFiller(ISettingsConverterChooser, IDictionary{string, IRawSettingsProvider}, IRawSettingsProvider)"/> constructor if it's used without specifying default provider.
        /// It's an instance of <see cref="Core.FromAppSettingsProvider"/>. If you want another provider to be used as default, set it here.
        /// Or even better - create an instance of <see cref="SettingsFiller"/> and pass whatever you want explicitly instead of changing static things.
        /// </summary>
        public static IRawSettingsProvider DefaultDefaultRawSettingsProvider { get; set; } = FromAppSettingsProvider;

        /// <summary>
        /// Raw settings providers which are passed by default to <see cref="SettingsFiller(ISettingsConverterChooser, IDictionary{string, IRawSettingsProvider}, IRawSettingsProvider)"/> constructor if it's used without specifying providers.
        /// Consists of <see cref="FromAppSettingsProvider"/> and <see cref="FromEnvironmentProvider"/>.
        /// </summary>
        public static IDictionary<string, IRawSettingsProvider> DefaultRawSettingsProviders { get; } = new Dictionary<string, IRawSettingsProvider> {
            { FromAppSettingsProviderKey, FromAppSettingsProvider},
            { FromEnvironmentProviderKey, FromEnvironmentProvider}
        };
    }
}
