﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidSettings.Core
{
    /// <summary>
    /// Provides basic converter chooser which only purpose is to choose proper converter from <see cref="Converters"/> and apply it on given value.
    /// </summary>
    public class SettingsConverterChooser : ISettingsConverterChooser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsConverterChooser"/> class with some <paramref name="converters"/>.
        /// </summary>
        /// <param name="converters">Initial list of converters to choose from.</param>
        public SettingsConverterChooser(IEnumerable<IRawSettingsConverter> converters)
        {
            this.Converters = converters != null ? new List<IRawSettingsConverter>(converters) : throw new RapidSettingsException($"{nameof(converters)} cannot be null!");
        }

        /// <summary>
        /// Converters which will be used to convert raw values.
        /// </summary>
        public ICollection<IRawSettingsConverter> Converters { get; } = new List<IRawSettingsConverter>();

        /// <summary>
        /// Chooses converter which <see cref="IRawSettingsConverter.CanConvert(Type, Type)"/> from <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>
        /// and returns result of its application on <paramref name="rawValue"/>.
        /// </summary>
        /// <typeparam name="TFrom">Type from which <paramref name="rawValue"/> should be converted.</typeparam>
        /// <typeparam name="TTo">Type to which <paramref name="rawValue"/> should be converted.</typeparam>
        /// <param name="rawValue">Raw value that needs conversion to type <typeparamref name="TTo"/>.</param>
        public TTo ChooseAndConvert<TFrom, TTo>(TFrom rawValue)
        {
            return (TTo)this.ChooseAndConvert(rawValue, typeof(TFrom), typeof(TTo));
        }

        private object ChooseAndConvert(object rawValue, Type sourceType, Type targetType)
        {
            if (rawValue == null)
            {
                throw new RapidSettingsException($"Setting's raw value is null and null cannot be converted!");
            }

            if (targetType.IsAssignableFrom(sourceType))
            {
                return rawValue;
            }

            var properConverter = this.Converters.FirstOrDefault(converter => converter.CanConvert(sourceType, targetType));
            if (properConverter == null)
            {
                throw new RapidSettingsException($"Suitable converter for given type {targetType} cannot be found in {nameof(this.Converters)}!");
            }

            var convertMethod = typeof(IRawSettingsConverter)
                .GetMethod(nameof(IRawSettingsConverter.Convert))
                .MakeGenericMethod(sourceType, targetType);

            try
            {
                return convertMethod.Invoke(properConverter, new[] { rawValue });
            }
            catch (Exception e)
            {
                var exceptionMessage = $"Required setting with raw value: \"{rawValue}\" cannot be converted to type {targetType} with converter of type {properConverter.GetType()}!";
                throw new RapidSettingsException(exceptionMessage, e);
            }
        }
    }
}
