using RapidSettings.Exceptions;
using RapidSettings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RapidSettings.Core
{
    /// <summary>
    /// Fills props/fields decorated with <see cref="ToFillAttribute"/>.
    /// </summary>
    public class SettingsFiller : ISettingsFillerSync, ISettingsFillerAsync
    {
        /// <summary>
        /// Settings providers by their names.
        /// </summary>
        public IDictionary<string, IRawSettingsProvider> RawSettingsProvidersByNames { get; } = new Dictionary<string, IRawSettingsProvider>();

        /// <summary>
        /// Converter chooser which will be used to choose converters which will be used for conversion ~ capt. Obvious.
        /// </summary>
        public ISettingsConverterChooser SettingsConverterChooser { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="SettingsFiller"/> class with converter chooser and default raw settings provider.
        /// </summary>
        public SettingsFiller(ISettingsConverterChooser settingsConverterChooser, IRawSettingsProvider defaultRawSettingsProvider)
            : this(
                  settingsConverterChooser,
                  new Dictionary<string, IRawSettingsProvider> {
                      { defaultRawSettingsProvider?.GetType().Name ?? throw new RapidSettingsException($"{nameof(defaultRawSettingsProvider)} cannot be null!"), defaultRawSettingsProvider }
                  }
            )
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SettingsFiller"/> class with converter chooser and some settings providers by their names.
        /// </summary>
        public SettingsFiller(ISettingsConverterChooser settingsConverterChooser, IDictionary<string, IRawSettingsProvider> rawSettingsProvidersByNames)
        {
            if (rawSettingsProvidersByNames == null || !rawSettingsProvidersByNames.Any())
            {
                throw new RapidSettingsException($"{nameof(rawSettingsProvidersByNames)} cannot be null or empty!");
            }

            var allSettingProvidersAreImplementingProperSubinterface = rawSettingsProvidersByNames.Values
                .All(provider =>
                    provider is IRawSettingsProviderSync
                    || provider is IRawSettingsProviderAsync);

            if (!allSettingProvidersAreImplementingProperSubinterface)
            {
                var providersWithoutNeededInterface = rawSettingsProvidersByNames.Values.Where(provider => !(provider is IRawSettingsProviderSync) || !(provider is IRawSettingsProviderAsync));
                var typesOfProvidersWithoutNeededInterface = providersWithoutNeededInterface.Select(provider => provider.GetType().Name);
                var exceptionMessage = $"Not every provider from {nameof(rawSettingsProvidersByNames)} is implementing proper interface ({nameof(IRawSettingsProviderSync)} or {nameof(IRawSettingsProviderAsync)})!";
                exceptionMessage += $"Their types are: {string.Join(", ", typesOfProvidersWithoutNeededInterface)}";
                throw new RapidSettingsException(exceptionMessage);
            }

            this.RawSettingsProvidersByNames = rawSettingsProvidersByNames;
            this.SettingsConverterChooser = settingsConverterChooser ?? throw new RapidSettingsException($"{nameof(settingsConverterChooser)} cannot be null!");
        }

        /// <summary>
        /// Fills <paramref name="objectWithDecoratedProps"/> properties decorated with <see cref="ToFillAttribute"/>.
        /// </summary>
        public void FillSettings<T>(T objectWithDecoratedProps)
        {
            var propertiesToFill = this.GetPropertiesToFill(typeof(T));
            foreach (var propToFill in propertiesToFill)
            {
                var setting = this.ResolveSetting<T>(propToFill);
                propToFill.SetValue(objectWithDecoratedProps, setting);
            }
        }

        /// <summary>
        /// Asynchronously fills <paramref name="objectWithDecoratedProps"/> properties decorated with <see cref="ToFillAttribute"/>.
        /// </summary>
        public async Task FillSettingsAsync<T>(T objectWithDecoratedProps)
        {
            var propertiesToFill = this.GetPropertiesToFill(typeof(T));
            foreach (var propToFill in propertiesToFill)
            {
                // TODO parallelization based on some setting 

                var setting = await this.ResolveSettingAsync<T>(propToFill);
                propToFill.SetValue(objectWithDecoratedProps, setting);
            }
        }

        private object ResolveSetting<T>(PropertyInfo propToFill)
        {
            #region ISetting support
            var isISetting = this.IsISetting(propToFill.PropertyType);
            var typeOfMemberToSet = isISetting ? this.GetUndelayingType(propToFill.PropertyType) : propToFill.PropertyType;
            #endregion

            #region Nullable support
            var underlayingTypeOfNullable = Nullable.GetUnderlyingType(typeOfMemberToSet);
            typeOfMemberToSet = underlayingTypeOfNullable ?? typeOfMemberToSet;
            #endregion

            #region Resolution
            var toFillAttribute = propToFill.GetCustomAttribute<ToFillAttribute>();
            var requestedRawSettingsProviderName = toFillAttribute.RawSettingsProviderName;
            var rawSettingsProvider = this.ChooseRawSettingsProvider(requestedRawSettingsProviderName);
            var rawSetting = this.GetRawSetting(rawSettingsProvider, toFillAttribute.Key);

            if (rawSetting == null && toFillAttribute.IsRequired)
            {
                throw new RapidSettingsException($"Resolution of required setting {propToFill.Name} returned null!");
            }
            #endregion

            #region Conversion

            // TODO make conversion optional (basing on some setting) when rawSetting's type is assignable to typeOfMemberToSet

            var convertMethod = typeof(ISettingsConverterChooser).GetMethod(nameof(ISettingsConverterChooser.ChooseAndConvert));
            var convertGenericMethod = convertMethod.MakeGenericMethod(rawSetting.GetType(), typeOfMemberToSet);

            bool hasValueSpecified;
            object settingValue = default(T);
            try
            {
                settingValue = convertGenericMethod.Invoke(this.SettingsConverterChooser, new[] { rawSetting });
                hasValueSpecified = true;
            }
            catch (Exception e)
            {
                if (toFillAttribute.IsRequired)
                {
                    throw new RapidSettingsException($"Conversion of required setting {propToFill.Name} failed!", e);
                }

                hasValueSpecified = false;
            }
            #endregion

            #region ISetting support
            object setting = settingValue;
            if (isISetting)
            {
                var settingMetadata = new SettingMetadata(toFillAttribute.Key, toFillAttribute.IsRequired, hasValueSpecified);
                setting = new Setting<T>((T)settingValue, settingMetadata);
            }
            #endregion

            return setting;
        }

        private async Task<object> ResolveSettingAsync<T>(PropertyInfo propToFill)
        {
            #region ISetting support
            var isISetting = this.IsISetting(propToFill.PropertyType);
            var typeOfMemberToSet = isISetting ? this.GetUndelayingType(propToFill.PropertyType) : propToFill.PropertyType;
            #endregion

            #region Nullable support
            var underlayingTypeOfNullable = Nullable.GetUnderlyingType(typeOfMemberToSet);
            typeOfMemberToSet = underlayingTypeOfNullable ?? typeOfMemberToSet;
            #endregion

            #region Resolution
            var toFillAttribute = propToFill.GetCustomAttribute<ToFillAttribute>();
            var requestedRawSettingsProviderName = toFillAttribute.RawSettingsProviderName;
            var rawSettingsProvider = this.ChooseRawSettingsProvider(requestedRawSettingsProviderName);
            var rawSetting = await this.GetRawSettingAsync(rawSettingsProvider, toFillAttribute.Key);

            if (rawSetting == null && toFillAttribute.IsRequired)
            {
                throw new RapidSettingsException($"Resolution of required setting {propToFill.Name} returned null!");
            }
            #endregion

            #region Conversion

            // TODO make conversion optional (basing on some setting) when rawSetting's type is assignable to typeOfMemberToSet

            var convertMethod = typeof(ISettingsConverterChooser).GetMethod(nameof(ISettingsConverterChooser.ChooseAndConvert));
            var convertGenericMethod = convertMethod.MakeGenericMethod(rawSetting.GetType(), typeOfMemberToSet);

            bool hasValueSpecified;
            object settingValue = default(T);
            try
            {
                settingValue = convertGenericMethod.Invoke(this.SettingsConverterChooser, new[] { rawSetting });
                hasValueSpecified = true;
            }
            catch (Exception e)
            {
                if (toFillAttribute.IsRequired)
                {
                    throw new RapidSettingsException($"Conversion of required setting {propToFill.Name} failed!", e);
                }

                hasValueSpecified = false;
            }
            #endregion

            #region ISetting support
            object setting = settingValue;
            if (isISetting)
            {
                var settingMetadata = new SettingMetadata(toFillAttribute.Key, toFillAttribute.IsRequired, hasValueSpecified);
                setting = new Setting<T>((T)settingValue, settingMetadata);
            }
            #endregion

            return setting;
        }

        private object GetRawSetting(IRawSettingsProvider rawSettingsProvider, string settingKey)
        {
            switch (rawSettingsProvider)
            {
                case IRawSettingsProviderAsync rawSettingsProviderAsync:
                    return rawSettingsProviderAsync.GetRawSettingAsync(settingKey).GetAwaiter().GetResult();
                case IRawSettingsProviderSync rawSettingsProviderSync:
                    return rawSettingsProviderSync.GetRawSetting(settingKey);
                default:
                    // should never be there
                    var errorMessage = $"{nameof(IRawSettingsProvider)} with type {rawSettingsProvider.GetType().Name} doesn't implement neither {nameof(IRawSettingsProviderSync)} nor {nameof(IRawSettingsProviderAsync)}!";
                    throw new RapidSettingsException(errorMessage);
            }
        }

        private async Task<object> GetRawSettingAsync(IRawSettingsProvider rawSettingsProvider, string settingKey)
        {
            switch (rawSettingsProvider)
            {
                case IRawSettingsProviderAsync rawSettingsProviderAsync:
                    return await rawSettingsProviderAsync.GetRawSettingAsync(settingKey);
                case IRawSettingsProviderSync rawSettingsProviderSync:
                    return rawSettingsProviderSync.GetRawSetting(settingKey);
                default:
                    // should never be there
                    var errorMessage = $"{nameof(IRawSettingsProvider)} with type {rawSettingsProvider.GetType().Name} doesn't implement neither {nameof(IRawSettingsProviderSync)} nor {nameof(IRawSettingsProviderAsync)}!";
                    throw new RapidSettingsException(errorMessage);
            }
        }

        private IRawSettingsProvider ChooseRawSettingsProvider(string requestedRawSettingsProviderName)
        {
            if (string.IsNullOrEmpty(requestedRawSettingsProviderName))
            {
                return this.RawSettingsProvidersByNames.Values.First();
            }

            if (!this.RawSettingsProvidersByNames.ContainsKey(requestedRawSettingsProviderName))
            {
                throw new RapidSettingsException($"Cannot find {nameof(IRawSettingsProvider)} by name {requestedRawSettingsProviderName} within {nameof(this.RawSettingsProvidersByNames)}!");
            }

            return this.RawSettingsProvidersByNames[requestedRawSettingsProviderName];
        }

        private IEnumerable<PropertyInfo> GetPropertiesToFill(Type typeOfObjectToFill)
        {
            // TODO respect nonpublic
            // TODO respect canwrite
            var propertiesToFill = typeOfObjectToFill.GetProperties().Where(prop => prop.IsDefined(typeof(ToFillAttribute), true));

            return propertiesToFill;
        }

        /// <summary>
        /// Gets underlaying <see cref="Type"/> of given <paramref name="propertyType"/>. Used to get generic type parameter from subclass of <see cref="ISetting{T}"/>.
        /// </summary>
        private Type GetUndelayingType(Type propertyType)
        {
            return propertyType.GetGenericArguments().First();
        }

        /// <summary>
        /// Checks if <paramref name="propertyType"/> implements <see cref="ISetting{T}"/>.
        /// </summary>
        /// <remarks>
        /// Used to determine if prop/field is wrapped in <see cref="ISetting{T}"/>.
        /// </remarks>
        private bool IsISetting(Type propertyType)
        {
            return propertyType.GetInterfaces().Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == typeof(ISetting<>));
        }
    }
}
