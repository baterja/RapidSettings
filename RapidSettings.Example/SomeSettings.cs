using RapidSettings.Core;
using System;

namespace RapidSettings.Example
{
    class SomeSettings
    {
        // this setting will be retrieved by key Host (default) with default provider
        // and if its retrieval or conversion will be impossible, exception will be thrown
        [ToFill]
        public Uri Host { get; private set; }

        // this setting will be retrieved by key Port (default) with default provider
        // but if its retrieval or conversion will be impossible, in Port.Value will be just a default int value (0)
        // and in Port.Metadata there will be few informations about it
        [ToFill(isRequired: false)]
        public Setting<int> Port { get; private set; }

        // this setting will be retrieved by key TMP with provider named ENV 
        // (which is the default key of FromEnvironmentProvider taken from SettingsFillerStaticDefaults)
        // but if its retrieval or conversion will be impossible, it will be just a default string value (null)
        [ToFill("TMP", isRequired: false, rawSettingsProviderName: SettingsFillerStaticDefaults.FromEnvironmentProviderKey)]
        public string TempFolderPath { get; private set; }
    }
}
