using RapidSettings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidSettings.Core
{
    /// <summary>
    /// Provides base class for easier creation of converters. Among others: unwrapping nullable types, checking for null value, choosing and applying proper Func.
    /// </summary>
    public abstract class RawSettingsConverterBase : IRawSettingsConverter
    {
        /// <summary>
        /// Dictionary of funcs that will be used to convert raw values from and to given type (but as <see cref="object"/>). 
        /// Type from and to which raw value should be converted should be given as key. Additionally type to which it should convert should be added as second func argument.
        /// </summary>
        protected IDictionary<(Type FromType, Type ToType), Func<object, Type, object>> ConvertingFuncs { get; } = new Dictionary<(Type FromType, Type ToType), Func<object, Type, object>>();

        /// <summary>
        /// Dictionary where key is a <see cref="Type"/> from which conversion is possible and value is a set of <see cref="Type"/>s to which the key type can be converted.
        /// </summary>
        protected IDictionary<Type, ISet<Type>> SupportedConversions { get; } = new Dictionary<Type, ISet<Type>>();

        /// <summary>
        /// Checks if conversion <paramref name="fromType"/> <paramref name="toType"/> is supported by this converter.
        /// </summary>
        /// <param name="fromType">Type from which support should be checked.</param>
        /// <param name="toType">Type to which support should be checked.</param>
        /// <returns>Should return true if there is any <see cref="Type"/> as a key in <see cref="SupportedConversions"/> to which <paramref name="fromType"/> is assignable
        /// and under that key there is a <see cref="Type"/> which is assignable to <paramref name="toType"/>.</returns>
        public virtual bool CanConvert(Type fromType, Type toType)
        {
            // gets underlaying type if type is nullable
            var underlayingFromType = Nullable.GetUnderlyingType(fromType) ?? fromType;

            if (this.SupportedConversions.ContainsKey(fromType) && this.SupportedConversions[fromType].Contains(toType))
            {
                return true;
            }

            var isGivenTypeAssignableToAnySupportedType = this.SupportedConversions.Keys
                .Any(supportedFromType => supportedFromType.IsAssignableFrom(underlayingFromType)
                    && this.SupportedConversions[supportedFromType].Any(supportedToType => toType.IsAssignableFrom(supportedToType)));

            return isGivenTypeAssignableToAnySupportedType;
        }

        /// <summary>
        /// Uses <see cref="ConvertingFuncs"/> to convert given object from type <typeparamref name="TFrom"/> to type <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TFrom">Type from which <paramref name="rawValue"/> should be converted.</typeparam>
        /// <typeparam name="TTo">Type on which <paramref name="rawValue"/> should be converted.</typeparam>
        /// <param name="rawValue">Value (possibly) convertible to <typeparamref name="TTo"/>.</param>
        /// <returns><typeparamref name="TTo"/> converted from <paramref name="rawValue"/> or exception if it was impossible.</returns>
        public virtual TTo Convert<TFrom, TTo>(TFrom rawValue)
        {
            if (rawValue == null)
            {
                throw new RapidSettingsException($"Null cannot be converted to anything!");
            }

            var typeOfT = typeof(TTo);
            var underlayingType = Nullable.GetUnderlyingType(typeOfT) ?? typeOfT;

            var value = default(TTo);
            try
            {
                var convertingFunc = this.GetConvertingFunc<TFrom, TTo>();
                value = (TTo)(object)convertingFunc((TFrom)rawValue, underlayingType);
            }
            catch (Exception e)
            {
                throw new RapidSettingsException($"{this.GetType().Name} couldn't successfully convert setting with value {rawValue} on type {underlayingType}.", e);
            }

            return value;
        }

        /// <summary>
        /// Get converting func which can convert from type to which <typeparamref name="TFrom"/> is assignable to type which is assignable to <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TFrom">Type from which value should be converted.</typeparam>
        /// <typeparam name="TTo">Type to which value should be converted.</typeparam>
        /// <returns>Should return proper func if there is any which can convert from type to which <typeparamref name="TFrom"/> is assignable 
        /// to type which is assignable to <typeparamref name="TTo"/>.</returns>
        protected virtual Func<object, Type, object> GetConvertingFunc<TFrom, TTo>()
        {
            var wantedKey = (typeof(TFrom), typeof(TTo));
            if (this.ConvertingFuncs.ContainsKey(wantedKey))
            {
                return this.ConvertingFuncs[(typeof(TFrom), typeof(TTo))];
            }

            return this.ConvertingFuncs.First(typesAndFunc => typesAndFunc.Key.FromType.IsAssignableFrom(typeof(TFrom)) && typeof(TTo).IsAssignableFrom(typesAndFunc.Key.ToType)).Value;
        }

        /// <summary>
        /// Adds converter support for pair of types - <paramref name="fromType"/> and <paramref name="toType"/> with use of function <paramref name="convertingFunc"/>.
        /// </summary>
        /// <param name="fromType">Type from which converting func can convert.</param>
        /// <param name="toType">Type to which converting func can convert.</param>
        /// <param name="convertingFunc">Function that will be used to convert raw value of type <paramref name="fromType"/> to type <paramref name="toType"/> (but as <see cref="object"/>).</param>
        protected void AddSupportForTypes(Type fromType, Type toType, Func<object, Type, object> convertingFunc)
        {
            this.ConvertingFuncs.Add((fromType, toType), convertingFunc);

            if (!this.SupportedConversions.ContainsKey(fromType))
            {
                this.SupportedConversions[fromType] = new HashSet<Type>();
            }

            if (!this.SupportedConversions[fromType].Add(toType))
            {
                throw new RapidSettingsException($"Converting func from type {fromType.Name} to type {toType.Name} is already added!");
            }
        }
    }
}
