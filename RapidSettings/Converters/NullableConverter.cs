using System;
using System.Reflection;

namespace RapidSettings.Core
{
    /// <summary>
    /// "Passthrough" converter which extracts value from <see cref="Nullable{T}"/> and passes it back to <see cref="ISettingsConverterChooser"/> for further conversion.
    /// </summary>
    public class NullableConverter : IRawSettingsConverter
    {
        private readonly ISettingsConverterChooser settingsConverterChooser;
        private readonly MethodInfo convertMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableConverter"/> class.
        /// </summary>
        /// <param name="settingsConverterChooser">Used to convert underlaying value of source <see cref="Nullable{T}"/> to desired type.</param>
        public NullableConverter(ISettingsConverterChooser settingsConverterChooser)
        {
            this.settingsConverterChooser = settingsConverterChooser ?? throw new ArgumentNullException(nameof(settingsConverterChooser));
            this.convertMethod = typeof(ISettingsConverterChooser).GetMethod(nameof(ISettingsConverterChooser.ChooseAndConvert));
        }

        /// <summary>
        /// Checks if conversion <paramref name="fromType"/> <paramref name="toType"/> is supported by this converter.
        /// </summary>
        /// <param name="fromType">Type from which support should be checked.</param>
        /// <param name="toType">Type to which support should be checked.</param>
        /// <returns>Should return true if <paramref name="toType"/> or <paramref name="toType"/> is <see cref="Nullable{T}"/>.</returns>
        public bool CanConvert(Type fromType, Type toType)
        {
            return Nullable.GetUnderlyingType(fromType) != null || Nullable.GetUnderlyingType(toType) != null;
        }

        /// <summary>
        /// Uses <see cref="settingsConverterChooser"/> received in the constructor to convert underlaying value of <paramref name="rawValue"/>
        /// to <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TFrom">Type from which <paramref name="rawValue"/> should be converted.</typeparam>
        /// <typeparam name="TTo">Type to which <paramref name="rawValue"/> should be converted.</typeparam>
        /// <param name="rawValue">Value (possibly) convertible to <typeparamref name="TTo"/>.</param>
        /// <returns><typeparamref name="TTo"/> converted from <paramref name="rawValue"/> or exception if it was impossible.</returns>
        public TTo Convert<TFrom, TTo>(TFrom rawValue)
        {
            var underlayingSourceNullableType = Nullable.GetUnderlyingType(typeof(TFrom));
            var extractedTargetType = Nullable.GetUnderlyingType(typeof(TTo)) ?? typeof(TTo);

            MethodInfo convertGenericMethod;
            if (underlayingSourceNullableType != null && rawValue == null)
            {
                convertGenericMethod = this.convertMethod.MakeGenericMethod(typeof(TFrom), extractedTargetType);

                return (TTo)convertGenericMethod.Invoke(this.settingsConverterChooser, new object[] { null });
            }

            convertGenericMethod = this.convertMethod.MakeGenericMethod(underlayingSourceNullableType, extractedTargetType);

            // boxing rawValue as an object is enough to unwrap underlaying struct value
            return (TTo)convertGenericMethod.Invoke(this.settingsConverterChooser, new object[] { rawValue });
        }
    }
}
