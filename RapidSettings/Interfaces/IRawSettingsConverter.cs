using System;

namespace RapidSettings.Interfaces
{
    /// <summary>
    /// Should <see cref="Convert{TFrom, TTo}(TFrom)"/> types which it declares that it <see cref="CanConvert(Type, Type)"/>.
    /// </summary>
    public interface IRawSettingsConverter
    {
        /// <summary>
        /// Checks if converter can convert values <paramref name="fromType"/> <paramref name="toType"/>.
        /// </summary>
        bool CanConvert(Type fromType, Type toType);

        /// <summary>
        /// Converts value from <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>.
        /// </summary>
        /// <param name="rawValue">Value which should be converted.</param>
        TTo Convert<TFrom, TTo>(TFrom rawValue);
    }
}
