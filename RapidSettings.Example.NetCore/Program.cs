using Microsoft.Extensions.Configuration;
using RapidSettings.Core;
using System;

namespace RapidSettings.Example.NetCore
{
    internal static class Program
    {
        private static void Main()
        {
            var settings = GetSettings();

            Console.WriteLine($"Host is \"{settings.Host}\"");

            Console.WriteLine($"Port is \"{settings.Port}\"");

            Console.WriteLine($"Your temporary folder's path is " + (string.IsNullOrEmpty(settings.TempFolderPath) ? "not in a TMP environment variable." : settings.TempFolderPath));

            Console.WriteLine("Try adding { \"Port\": 1234 } value to appsettings.json and run an example again.");

            Console.ReadLine();
        }

        private static SomeSettings GetSettings()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var settingsFiller = new SettingsFiller(
                settingsConverterChooser: SettingsFillerStaticDefaults.DefaultSettingsConverterChooser,
                rawSettingsProvidersByNames: SettingsFillerStaticDefaults.DefaultRawSettingsProviders,
                defaultRawSettingsProvider: new FromIConfigurationProvider(configuration));

            return settingsFiller.CreateWithSettings<SomeSettings>();
        }
    }
}
