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
        /// Settings providers by their names.
        /// </summary>
        public IDictionary<string, IRawSettingsProvider> RawSettingsProvidersByNames { get; }

        /// <summary>
        /// Converter chooser which will be used to choose converters which will be used for conversion ~ capt. Obvious.
        /// </summary>
        public ISettingsConverterChooser SettingsConverterChooser { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="SettingsFiller"/> class with converter chooser and default raw settings provider. Any parameter left with null will default to values from <see cref="SettingsFillerStaticDefaults"/>.
        /// </summary>
        public SettingsFiller(ISettingsConverterChooser settingsConverterChooser = null, IRawSettingsProvider defaultRawSettingsProvider = null)
            : this(
                  settingsConverterChooser ?? SettingsFillerStaticDefaults.DefaultSettingsConverterChooser,
                  defaultRawSettingsProvider == null ? new Dictionary<string, IRawSettingsProvider>(SettingsFillerStaticDefaults.DefaultRawSettingsProviders) : new Dictionary<string, IRawSettingsProvider> { { DefaultRawSettingsProviderKey, defaultRawSettingsProvider } },
                  defaultRawSettingsProvider ?? SettingsFillerStaticDefaults.DefaultDefaultRawSettingsProvider
              )
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SettingsFiller"/> class with converter chooser, some settings providers by their names and optionally a default <see cref="IRawSettingsProvider"/>.
        /// </summary>
        public SettingsFiller(ISettingsConverterChooser settingsConverterChooser, IDictionary<string, IRawSettingsProvider> rawSettingsProvidersByNames, IRawSettingsProvider defaultRawSettingsProvider = null)
        {
            this.SettingsConverterChooser = settingsConverterChooser ?? throw new RapidSettingsException($"{nameof(settingsConverterChooser)} cannot be null!");

            if (rawSettingsProvidersByNames == null || !rawSettingsProvidersByNames.Any())
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
        /// Fills <paramref name="objectWithDecoratedProps"/> properties decorated with <see cref="ToFillAttribute"/>.
        /// </summary>
        public void FillSettings<T>(T objectWithDecoratedProps)
        {
            var propertiesToFill = this.GetPropertiesToFill(typeof(T));
            foreach (var propToFill in propertiesToFill)
            {
                this.ValidateIfPropertyIsFillable(propToFill);

                var setting = this.ResolveSetting(propToFill);
                propToFill.SetValue(objectWithDecoratedProps, setting);
            }
        }

        // TODO split
        private object ResolveSetting(PropertyInfo propToFill)
        {
            var typeOfMemberToSet = propToFill.PropertyType;

            #region Nullable support
            var underlayingTypeOfNullable = Nullable.GetUnderlyingType(typeOfMemberToSet);
            var unwrappedTypeOfPropToSet = underlayingTypeOfNullable ?? typeOfMemberToSet;
            #endregion

            #region Resolution
            var toFillAttribute = propToFill.GetCustomAttribute<ToFillAttribute>();
            var requestedRawSettingsProviderName = toFillAttribute.RawSettingsProviderName;
            var rawSettingsProvider = this.ChooseRawSettingsProvider(requestedRawSettingsProviderName);
            var rawSetting = rawSettingsProvider.GetRawSetting(toFillAttribute.Key);

            object settingValue = typeOfMemberToSet.IsValueType && underlayingTypeOfNullable == null ? Activator.CreateInstance(typeOfMemberToSet) : null;
            var hasValueSpecified = rawSetting != null;
            if (!hasValueSpecified)
            {
                if (toFillAttribute.IsRequired)
                {
                    throw new RapidSettingsException($"Resolution of required setting {propToFill.Name} returned null!");
                }

                return settingValue;
            }
            #endregion

            #region Conversion

            // TODO make conversion optional (basing on some setting) when rawSetting's type is assignable to typeOfMemberToSet

            var convertMethod = typeof(ISettingsConverterChooser).GetMethod(nameof(ISettingsConverterChooser.ChooseAndConvert));
            var convertGenericMethod = convertMethod.MakeGenericMethod(rawSetting.GetType(), unwrappedTypeOfPropToSet);

            try
            {
                settingValue = convertGenericMethod.Invoke(this.SettingsConverterChooser, new[] { rawSetting });
            }
            catch (Exception e)
            {
                if (toFillAttribute.IsRequired)
                {
                    throw new RapidSettingsException($"Conversion of required setting {propToFill.Name} failed!", e);
                }
            }
            #endregion

            return settingValue;
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

        private void ValidateIfPropertyIsFillable(PropertyInfo propertyToFill)
        {
            if (!propertyToFill.CanWrite)
            {
                throw new RapidSettingsException($"Property {propertyToFill.Name} on type {propertyToFill.DeclaringType} doesn't have a setter!");
            }
        }

        private IEnumerable<PropertyInfo> GetPropertiesToFill(Type typeOfObjectToFill)
        {
            var propertiesToFill = typeOfObjectToFill
                .GetProperties()
                .Where(prop => prop.IsDefined(typeof(ToFillAttribute), true));

            return propertiesToFill;
        }
    }
}
