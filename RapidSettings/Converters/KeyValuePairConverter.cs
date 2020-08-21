using System;
using System.Collections.Generic;
using System.Reflection;

namespace RapidSettings.Core
{
    /// <summary>
    /// "Passthrough" converter which extracts key and value from <see cref="KeyValuePair{TKey, TValue}"/> and passes them back to <see cref="ISettingsConverterChooser"/> for further conversion.
    /// </summary>
    public class KeyValuePairConverter : IRawSettingsConverter
    {
        private readonly ISettingsConverterChooser settingsConverterChooser;
        private readonly MethodInfo convertMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairConverter"/> class.
        /// </summary>
        /// <param name="settingsConverterChooser">Used to convert underlaying key and value of source <see cref="KeyValuePair{TKey, TValue}"/> to desired types.</param>
        public KeyValuePairConverter(ISettingsConverterChooser settingsConverterChooser)
        {
            this.settingsConverterChooser = settingsConverterChooser ?? throw new ArgumentNullException(nameof(settingsConverterChooser));
            this.convertMethod = typeof(ISettingsConverterChooser).GetMethod(nameof(ISettingsConverterChooser.ChooseAndConvert));
        }

        /// <summary>
        /// Checks if conversion <paramref name="fromType"/> <paramref name="toType"/> is supported by this converter.
        /// </summary>
        /// <param name="fromType">Type from which support should be checked.</param>
        /// <param name="toType">Type to which support should be checked.</param>
        /// <returns>Should return true if both <paramref name="fromType"/> and <paramref name="toType"/> are <see cref="KeyValuePair{TKey, TValue}"/></returns>
        public bool CanConvert(Type fromType, Type toType)
        {
            var isTargetTypeKvp = toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
            var isSourceTypeKvp = fromType.IsGenericType && fromType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);

            return isTargetTypeKvp && isSourceTypeKvp;
        }

        /// <summary>
        /// Uses <see cref="settingsConverterChooser"/> received in the constructor to convert underlaying key and value of <paramref name="rawKeyValuePair"/> 
        /// to types of key and value of <typeparamref name="TTo"/> (which also have to be of type <see cref="KeyValuePair{TKey, TValue}"/>).
        /// </summary>
        /// <typeparam name="TFrom">Type from which <paramref name="rawKeyValuePair"/> should be converted.</typeparam>
        /// <typeparam name="TTo">Type to which <paramref name="rawKeyValuePair"/> should be converted.</typeparam>
        /// <param name="rawKeyValuePair">Value (possibly) convertible to <typeparamref name="TTo"/>.</param>
        /// <returns><typeparamref name="TTo"/> converted from <paramref name="rawKeyValuePair"/> or exception if it was impossible.</returns>
        public TTo Convert<TFrom, TTo>(TFrom rawKeyValuePair)
        {
            var targetType = typeof(TTo);
            var targetKeyValuePairGenericArguments = targetType.GetGenericArguments();
            var targetKeyType = targetKeyValuePairGenericArguments[0];
            var targetValueType = targetKeyValuePairGenericArguments[1];

            var sourceKeyValuePairType = this.GetSourceKeyValuePairType(rawKeyValuePair);

            var key = this.ConvertKey(sourceKeyValuePairType, rawKeyValuePair, targetKeyType);
            var value = this.ConvertValue(sourceKeyValuePairType, rawKeyValuePair, targetValueType);

            var targetKeyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(targetKeyType, targetValueType);
            var keyValuePair = Activator.CreateInstance(targetKeyValuePairType, key, value);
            return (TTo)keyValuePair;
        }

        private object ConvertKey(Type sourceKeyValuePairType, object rawKeyValuePair, Type targetKeyType)
        {
            return this.ConvertProperty(sourceKeyValuePairType, rawKeyValuePair, nameof(KeyValuePair<object, object>.Key), targetKeyType);
        }

        private object ConvertValue(Type sourceKeyValuePairType, object rawKeyValuePair, Type targetValueType)
        {
            return this.ConvertProperty(sourceKeyValuePairType, rawKeyValuePair, nameof(KeyValuePair<object, object>.Value), targetValueType);
        }

        private object ConvertProperty(Type sourceKeyValuePairType, object rawKeyValuePair, string propertyName, Type targetType)
        {
            var propertyDefinition = sourceKeyValuePairType.GetProperty(propertyName);
            var rawPropertyValue = propertyDefinition.GetValue(rawKeyValuePair);
            var convertGenericMethod = this.convertMethod.MakeGenericMethod(rawPropertyValue.GetType(), targetType);
            var convertedPropertyValue = convertGenericMethod.Invoke(this.settingsConverterChooser, new[] { rawPropertyValue });

            return convertedPropertyValue;
        }

        private Type GetSourceKeyValuePairType(object rawKeyValuePair)
        {
            var sourceKeyValuePairGenericArguments = rawKeyValuePair.GetType().GetGenericArguments();
            var sourceKeyType = sourceKeyValuePairGenericArguments[0];
            var sourceValueType = sourceKeyValuePairGenericArguments[1];
            var sourceKeyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(sourceKeyType, sourceValueType);

            return sourceKeyValuePairType;
        }
    }
}
