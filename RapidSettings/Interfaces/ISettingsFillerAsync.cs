using RapidSettings.Core;
using System.Threading.Tasks;

namespace RapidSettings.Interfaces
{
    /// <summary>
    /// Interface for a class which asynchronously fills members decorated <see cref="ToFillAttribute"/> of given class instance.
    /// </summary>
    public interface ISettingsFillerAsync : ISettingsFiller
    {
        /// <summary>
        /// Asynchronously fills members decorated <see cref="ToFillAttribute"/> of <paramref name="objectToFill"/>.
        /// </summary>
        /// <param name="objectToFill">Object instance which decorated members should be filled.</param>
        Task FillSettingsAsync<T>(T objectToFill);
    }

    /// <summary>
    /// Provides extension method for <see cref="ISettingsFillerAsync"/> which creates instance of a class which should be filled.
    /// </summary>
    public static class ISettingsFillerAsyncExtensions
    {
        /// <summary>
        /// Creates new instance of a class <typeparamref name="T"/> and asynchronously fill its members which are decorated with <see cref="ToFillAttribute"/>.
        /// </summary>
        public static async Task<T> CreateWithSettingsAsync<T>(this ISettingsFillerAsync settingsFiller) where T : new()
        {
            var settingsClass = new T();
            await settingsFiller.FillSettingsAsync(settingsClass);
            return settingsClass;
        }
    }
}
