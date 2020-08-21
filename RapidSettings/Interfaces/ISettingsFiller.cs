using System;

namespace RapidSettings.Core
{
    /// <summary>
    /// Interface for a class which fills members decorated <see cref="ToFillAttribute"/> of given class instance.
    /// </summary>
    public interface ISettingsFiller
    {
        /// <summary>
        /// Fills members decorated <see cref="ToFillAttribute"/> of <paramref name="objectToFill"/>.
        /// </summary>
        /// <param name="objectToFill">Object instance which decorated members should be filled.</param>
        void FillSettings<T>(T objectToFill);
    }

    /// <summary>
    /// Provides extension method for <see cref="ISettingsFiller"/> which creates instance of a class which should be filled.
    /// </summary>
    public static class ISettingsFillerExtensions
    {
        /// <summary>
        /// Creates new instance of a class <typeparamref name="T"/> and fill its members which are decorated with <see cref="ToFillAttribute"/>.
        /// </summary>
        /// <param name="settingsFiller">Filler to use.</param>
        public static T CreateWithSettings<T>(this ISettingsFiller settingsFiller)
            where T : new()
        {
            if (settingsFiller is null)
            {
                throw new ArgumentNullException(nameof(settingsFiller));
            }

            var settingsClass = new T();
            settingsFiller.FillSettings(settingsClass);
            return settingsClass;
        }
    }
}