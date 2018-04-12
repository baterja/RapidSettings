using RapidSettings.Core;
using System;
using System.Collections.Generic;

namespace RapidSettings.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = GetSettings();

            Console.WriteLine($"Host is \"{settings.Host}\"");

            Console.Write($"Port is \"{settings.Port}\"");
            if (!settings.Port.Metadata.IsRequired)
            {
                Console.Write($" but it wasn't required setting so its value might be just a default {settings.Port.Value.GetType()} value");
                if (settings.Port.Metadata.HasValueSpecified)
                {
                    Console.WriteLine($" but its value was specified actually.");
                }
                else
                {
                    Console.WriteLine($" and it really is just a default value.");
                }
            }
            else
            {
                Console.WriteLine($" and its value was surely specified because it's required.");
            }

            Console.WriteLine($"Your temporary folder's path is " + (string.IsNullOrEmpty(settings.TempFolderPath) ? "not in a TMP environment variable." : settings.TempFolderPath));

            Console.ReadLine();
        }

        private static SomeSettings GetSettings()
        {
            var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });
            var providersByNames = new Dictionary<string, IRawSettingsProvider> {
                { SomeSettings.FromEnvironmentProviderName, new FromEnvironmentProvider() },
                { SomeSettings.FromAppSettingsProviderName, new FromAppSettingsProvider() }
            };
            var settingsFiller = new SettingsFiller(converterChooser, providersByNames);

            return settingsFiller.CreateWithSettings<SomeSettings>();
        }
    }
}
