using RapidSettings.Core;
using System;

namespace RapidSettings.Example
{
    internal class Program
    {
        private static void Main()
        {
            var settings = GetSettings();

            Console.WriteLine($"Host is \"{settings.Host}\"");

            Console.WriteLine($"Port is \"{settings.Port}\"");

            Console.WriteLine($"Your temporary folder's path is " + (string.IsNullOrEmpty(settings.TempFolderPath) ? "not in a TMP environment variable." : settings.TempFolderPath));

            Console.ReadLine();
        }

        private static SomeSettings GetSettings()
        {
            var settingsFiller = new SettingsFiller();

            return settingsFiller.CreateWithSettings<SomeSettings>();
        }
    }
}
