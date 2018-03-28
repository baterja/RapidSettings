using System.Collections.Generic;

namespace RapidSettings.Interfaces
{
    /// <summary>
    /// Provides interface for a class that chooses one of its <see cref="Converters"/> to <see cref="ChooseAndConvert{TFrom, TTo}(TFrom)"/> properly.
    /// </summary>
    public interface ISettingsConverterChooser
    {
        /// <summary>
        /// Converters that are used by this chooser to convert raw value of a setting to its target type.
        /// </summary>
        /// <remarks>
        /// If many converters declare support for some type, there is no guarantee which one will be used,
        /// so please adjust your converters <see cref="IRawSettingsConverter.CanConvert(System.Type, System.Type)"/>.
        /// </remarks>
        IList<IRawSettingsConverter> Converters { get; }

        /// <summary>
        /// Uses <see cref="Converters"/> to convert <paramref name="rawValue"/> from type <typeparamref name="TFrom"/> to type <typeparamref name="TTo"/>.
        /// </summary>
        /// <param name="rawValue">Raw value to convert.</param>
        TTo ChooseAndConvert<TFrom, TTo>(TFrom rawValue);
    }
}
