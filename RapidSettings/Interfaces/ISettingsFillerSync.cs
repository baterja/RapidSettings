namespace RapidSettings.Interfaces
{
    using RapidSettings.Core;

    /// <summary>
    /// Interface for a class which fills members decorated <see cref="ToFillAttribute"/> of given class instance.
    /// </summary>
    public interface ISettingsFillerSync : ISettingsFiller
    {
        /// <summary>
        /// Fills members decorated <see cref="ToFillAttribute"/> of <paramref name="objectToFill"/>.
        /// </summary>
        /// <param name="objectToFill">Object instance which decorated members should be filled.</param>
        void FillSettings<T>(T objectToFill);
    }
}

namespace RapidSettings.Core
{
    using RapidSettings.Interfaces;

    /// <summary>
    /// Provides extension method for <see cref="ISettingsFillerSync"/> which creates instance of a class which should be filled.
    /// </summary>
    public static class ISettingsFillerSyncExtensions
    {
        /// <summary>
        /// Creates new instance of a class <typeparamref name="T"/> and fill its members which are decorated with <see cref="ToFillAttribute"/>.
        /// </summary>
        public static T CreateWithSettings<T>(this ISettingsFillerSync settingsFiller) where T : new()
        {
            var settingsClass = new T();
            settingsFiller.FillSettings(settingsClass);
            return settingsClass;
        }
    }
}