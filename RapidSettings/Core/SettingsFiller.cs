using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RapidSettings.Core
{
    /// <summary>
    /// Fills props decorated with <see cref="ToFillAttribute"/>.
    /// </summary>
    public class SettingsFiller : ISettingsFiller
    {
        /// <summary>
        /// Key which will should be used as key in <see cref="RawSettingsProvidersByNames"/> to register default provider.
        /// </summary>
        public const string DefaultRawSettingsProviderKey = "###DEFAULT###";

        /// <summary>
        /// Initializes a new instance of <see cref="SettingsFiller"/> class with converter chooser and default raw settings provider. Any parameter left with null will default to values from <see cref="SettingsFillerStaticDefaults"/>.
        /// </summary>
        public SettingsFiller(ISettingsConverterChooser settingsConverterChooser = null, IRawSettingsProvider defaultRawSettingsProvider = null)
            : this(
                  settingsConverterChooser ?? SettingsFillerStaticDefaults.DefaultSettingsConverterChooser,
                  defaultRawSettingsProvider == null ? new Dictionary<string, IRawSettingsProvider>(SettingsFillerStaticDefaults.DefaultRawSettingsProviders) : new Dictionary<string, IRawSettingsProvider> { { DefaultRawSettingsProviderKey, defaultRawSettingsProvider } },
                  defaultRawSettingsProvider ?? SettingsFillerStaticDefaults.DefaultDefaultRawSettingsProvider)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SettingsFiller"/> class with converter chooser, some settings providers by their names and optionally a default <see cref="IRawSettingsProvider"/>.
        /// </summary>
        public SettingsFiller(ISettingsConverterChooser settingsConverterChooser, IDictionary<string, IRawSettingsProvider> rawSettingsProvidersByNames, IRawSettingsProvider defaultRawSettingsProvider = null)
        {
            this.SettingsConverterChooser = settingsConverterChooser ?? throw new RapidSettingsException($"{nameof(settingsConverterChooser)} cannot be null!");

            if (rawSettingsProvidersByNames == null || rawSettingsProvidersByNames.Count == 0)
            {
                throw new RapidSettingsException($"{nameof(rawSettingsProvidersByNames)} cannot be null or empty!");
            }

            this.RawSettingsProvidersByNames = rawSettingsProvidersByNames;

            if (defaultRawSettingsProvider != null)
            {
                this.RawSettingsProvidersByNames[DefaultRawSettingsProviderKey] = defaultRawSettingsProvider;
            }
        }

        /// <summary>
        /// Settings providers by their names.
        /// </summary>
        public IDictionary<string, IRawSettingsProvider> RawSettingsProvidersByNames { get; }

        /// <summary>
        /// Converter chooser which will be used to choose converters which will be used for conversion ~ capt. Obvious.
        /// </summary>
        public ISettingsConverterChooser SettingsConverterChooser { get; }

        /// <summary>
        /// Fills <paramref name="objectToFill"/> properties decorated with <see cref="ToFillAttribute"/>.
        /// </summary>
        public void FillSettings<T>(T objectToFill)
        {
            var propertiesToFill = GetPropertiesToFill(typeof(T));
            foreach (var propToFill in propertiesToFill)
            {
                ValidateIfPropertyIsFillable(propToFill);

                var setting = this.ResolveSetting(propToFill);
                propToFill.SetValue(objectToFill, setting);
            }
        }

        private static void ValidateIfPropertyIsFillable(PropertyInfo propertyToFill)
        {
            if (!propertyToFill.CanWrite)
            {
                throw new RapidSettingsException($"Property {propertyToFill.Name} on type {propertyToFill.DeclaringType} doesn't have a setter!");
            }
        }

        private static IEnumerable<PropertyInfo> GetPropertiesToFill(Type typeOfObjectToFill)
        {
            var propertiesToFill = typeOfObjectToFill
                .GetProperties()
                .Where(prop => prop.IsDefined(typeof(ToFillAttribute), true));

            return propertiesToFill;
        }

        private object ResolveSetting(PropertyInfo propToFill)
        {
            var typeOfMemberToSet = propToFill.PropertyType;

            var toFillAttribute = propToFill.GetCustomAttribute<ToFillAttribute>();
            var requestedRawSettingsProviderName = toFillAttribute.RawSettingsProviderName;
            var rawSettingsProvider = this.ChooseRawSettingsProvider(requestedRawSettingsProviderName);
            var rawSetting = rawSettingsProvider.GetRawSetting(toFillAttribute.Key);

            var settingValue = typeOfMemberToSet.IsValueType ? Activator.CreateInstance(typeOfMemberToSet) : null;
            var hasValueSpecified = rawSetting != null;
            if (!hasValueSpecified)
            {
                if (toFillAttribute.IsRequired)
                {
                    throw new RapidSettingsException($"Resolution of required setting {propToFill.Name} returned null!");
                }

                return settingValue;
            }

            try
            {
                settingValue = this.Convert(rawSetting, typeOfMemberToSet);
            }
            catch (Exception e)
            {
                if (toFillAttribute.IsRequired)
                {
                    throw new RapidSettingsException($"Conversion of required setting {propToFill.Name} failed!", e);
                }
            }

            return settingValue;
        }

        private object Convert(object rawValue, Type targetType)
        {
            var convertMethod = typeof(ISettingsConverterChooser).GetMethod(nameof(ISettingsConverterChooser.ChooseAndConvert));
            var convertGenericMethod = convertMethod.MakeGenericMethod(rawValue.GetType(), targetType);

            return convertGenericMethod.Invoke(this.SettingsConverterChooser, new[] { rawValue });
        }

        private IRawSettingsProvider ChooseRawSettingsProvider(string requestedRawSettingsProviderName)
        {
            if (string.IsNullOrEmpty(requestedRawSettingsProviderName))
            {
                if (!this.RawSettingsProvidersByNames.ContainsKey(DefaultRawSettingsProviderKey))
                {
                    var errorMessage = $"Cannot find default {nameof(IRawSettingsProvider)} by (default) key {DefaultRawSettingsProviderKey} within {nameof(this.RawSettingsProvidersByNames)}!";
                    errorMessage += $" (for most cases you should use proper argument in constructor or you can manually add a provider with key {DefaultRawSettingsProviderKey} to {nameof(this.RawSettingsProvidersByNames)})";
                    throw new RapidSettingsException(errorMessage);
                }

                return this.RawSettingsProvidersByNames[DefaultRawSettingsProviderKey];
            }

            if (!this.RawSettingsProvidersByNames.ContainsKey(requestedRawSettingsProviderName))
            {
                throw new RapidSettingsException($"Cannot find {nameof(IRawSettingsProvider)} by name {requestedRawSettingsProviderName} within {nameof(this.RawSettingsProvidersByNames)}!");
            }

            return this.RawSettingsProvidersByNames[requestedRawSettingsProviderName];
        }
    }
}
