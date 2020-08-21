using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RapidSettings.Core
{
    /// <summary>
    /// "Passthrough" converter which extracts elements from <see cref="IEnumerable{T}"/> and passes them back to <see cref="ISettingsConverterChooser"/> for further conversion.
    /// Able to create <see cref="List{T}"/>, <see cref="HashSet{T}"/> and <see cref="Dictionary{TKey, TValue}"/>.
    /// </summary>
    public class EnumerableConverter : IRawSettingsConverter
    {
        private readonly ISettingsConverterChooser settingsConverterChooser;
        private readonly MethodInfo convertMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableConverter"/> class.
        /// </summary>
        /// <param name="settingsConverterChooser">Used to convert child elements of source <see cref="IEnumerable{T}"/> to desired types.</param>
        public EnumerableConverter(ISettingsConverterChooser settingsConverterChooser)
        {
            this.settingsConverterChooser = settingsConverterChooser ?? throw new ArgumentNullException(nameof(settingsConverterChooser));
            this.convertMethod = typeof(ISettingsConverterChooser).GetMethod(nameof(ISettingsConverterChooser.ChooseAndConvert));
        }

        /// <summary>
        /// Checks if conversion <paramref name="fromType"/> <paramref name="toType"/> is supported by this converter.
        /// </summary>
        /// <param name="fromType">Type from which support should be checked.</param>
        /// <param name="toType">Type to which support should be checked.</param>
        /// <returns>Should return true if <paramref name="fromType"/> is <see cref="IEnumerable{T}"/> 
        /// and <see cref="List{T}"/>, <see cref="HashSet{T}"/> or <see cref="Dictionary{TKey, TValue}"/> is assignable to <paramref name="toType"/>.</returns>
        public bool CanConvert(Type fromType, Type toType)
        {
            var isSourceTypeGenericIEnumerable = this.IsOrInheritsGenericIEnumerable(fromType, out _);
            if (!isSourceTypeGenericIEnumerable)
            {
                return false;
            }

            var isTargetTypeGenericIEnumerable = this.IsOrInheritsGenericIEnumerable(toType, out var targetEnumerableUnderlayingType);
            if (!isTargetTypeGenericIEnumerable)
            {
                return false;
            }

            var canCreateTypeAssignableToTarget = this.GetAssignableCollectionFactory(toType, targetEnumerableUnderlayingType) != null;
            return canCreateTypeAssignableToTarget;
        }

        /// <summary>
        /// Uses <see cref="settingsConverterChooser"/> received in the constructor to convert elements of <paramref name="rawValue"/> 
        /// to underlaying type of <typeparamref name="TTo"/> and creates <see cref="List{T}"/>, <see cref="HashSet{T}"/> or 
        /// <see cref="Dictionary{TKey, TValue}"/> (whichever is assignable to <typeparamref name="TTo"/>) filled with those values.
        /// </summary>
        /// <typeparam name="TFrom">Type from which <paramref name="rawValue"/> should be converted.</typeparam>
        /// <typeparam name="TTo">Type to which <paramref name="rawValue"/> should be converted.</typeparam>
        /// <param name="rawValue">Value (possibly) convertible to <typeparamref name="TTo"/>.</param>
        /// <returns><typeparamref name="TTo"/> converted from <paramref name="rawValue"/> or exception if it was impossible.</returns>
        public TTo Convert<TFrom, TTo>(TFrom rawValue)
        {
            if (!this.IsOrInheritsGenericIEnumerable(typeof(TTo), out var targetEnumerableUnderlayingType))
            {
                throw new RapidSettingsException($"{nameof(EnumerableConverter)} is unable to convert to type {typeof(TTo).Name} as it doesn't implement generic IEnumerable<>!");
            }

            var enumerableRawValues = (IEnumerable)rawValue;
            var convertedValues = this.ConvertValues(enumerableRawValues, targetEnumerableUnderlayingType);

            var collection = this.CreateSimilarCollection(typeof(TTo), targetEnumerableUnderlayingType, convertedValues);

            return (TTo)collection;
        }

        private IList ConvertValues(IEnumerable rawValues, Type targetEnumerableUnderlayingType)
        {
            var convertedList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(targetEnumerableUnderlayingType));
            foreach (var rawValue in rawValues)
            {
                var convertGenericMethod = this.convertMethod.MakeGenericMethod(rawValue.GetType(), targetEnumerableUnderlayingType);
                convertedList.Add(convertGenericMethod.Invoke(this.settingsConverterChooser, new[] { rawValue }));
            }

            return convertedList;
        }

        private IEnumerable CreateSimilarCollection(Type targetCollectionType, Type targetEnumerableUnderlayingType, IList convertedValues)
        {
            var assignableCollectionFactory = this.GetAssignableCollectionFactory(targetCollectionType, targetEnumerableUnderlayingType);
            if (assignableCollectionFactory == null)
            {
                var errorMessage = $"{nameof(EnumerableConverter)} is unable to create type of collection assignable to target collection type which is: {targetCollectionType.Name}." +
                    $" {nameof(EnumerableConverter)} is able to create only List<>, HashSet<> or Dictionary<,> and those types are not assignable to target collection type.";
                throw new RapidSettingsException(errorMessage);
            }

            return assignableCollectionFactory(convertedValues);
        }

        private Func<IList, IEnumerable> GetAssignableCollectionFactory(Type targetCollectionType, Type targetEnumerableUnderlayingType)
        {
            if (targetCollectionType.IsAssignableFrom(typeof(List<>).MakeGenericType(targetEnumerableUnderlayingType)))
            {
                var listConstructor = typeof(List<>)
                    .MakeGenericType(targetEnumerableUnderlayingType)
                    .GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(targetEnumerableUnderlayingType) });

                return values => (IEnumerable)listConstructor.Invoke(new[] { values });
            }

            if (targetCollectionType.IsAssignableFrom(typeof(HashSet<>).MakeGenericType(targetEnumerableUnderlayingType)))
            {
                var hashSetConstructor = typeof(HashSet<>)
                    .MakeGenericType(targetEnumerableUnderlayingType)
                    .GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(targetEnumerableUnderlayingType) });

                return values => (IEnumerable)hashSetConstructor.Invoke(new[] { values });
            }

            if (targetEnumerableUnderlayingType.IsGenericType 
                && targetEnumerableUnderlayingType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var keyValuePairGenericArguments = targetEnumerableUnderlayingType.GetGenericArguments();
                if (targetCollectionType.IsAssignableFrom(typeof(Dictionary<,>).MakeGenericType(keyValuePairGenericArguments)))
                {
                    var dictType = typeof(Dictionary<,>).MakeGenericType(targetCollectionType.GetGenericArguments());
                    var dictConstructor = dictType.GetConstructor(Type.EmptyTypes);

                    var dict = dictConstructor.Invoke(new object[0]);
                    var addMethod = dictType.GetMethod(nameof(Dictionary<object, object>.Add));

                    var keyProperty = targetEnumerableUnderlayingType.GetProperty(nameof(KeyValuePair<object, object>.Key));
                    var valueProperty = targetEnumerableUnderlayingType.GetProperty(nameof(KeyValuePair<object, object>.Value));

                    return values =>
                    {
                        foreach (var keyValuePair in values)
                        {
                            var key = keyProperty.GetValue(keyValuePair);
                            var value = valueProperty.GetValue(keyValuePair);
                            addMethod.Invoke(dict, new[] { key, value });
                        }

                        return (IEnumerable)dict;
                    };
                }
            }

            return null;
        }

        private bool IsOrInheritsGenericIEnumerable(Type type, out Type enumerableUnderlayingType)
        {
            var ienumerableType = typeof(IEnumerable<>);
            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == ienumerableType)
            {
                enumerableUnderlayingType = type.GetGenericArguments().Single();
                return true;
            }

            enumerableUnderlayingType = type
                .GetInterfaces()
                .FirstOrDefault(implementedInterface => implementedInterface.IsGenericType && implementedInterface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ?.GetGenericArguments()
                .Single();
            return enumerableUnderlayingType != null;
        }
    }
}
