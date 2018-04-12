using RapidSettings.Core;
using System;

namespace RapidSettings.Example
{
    class SomeSettings
    {
        public const string FromEnvironmentProviderName = "env";
        public const string FromAppSettingsProviderName = "config";

        // this setting will be retrieved by key Host (default) with provider named "config" 
        // and if its retrieval or conversion will be impossible, exception will be thrown
        [ToFill(rawSettingsProviderName: FromAppSettingsProviderName)]
        public Uri Host { get; private set; }

        // this setting will be retrieved by key Port (default) with provider named "config" 
        // but if its retrieval or conversion will be impossible, in Port.Value will be just a default int value (0)
        // and in Port.Metadata there will be few informations about it
        [ToFill(isRequired: false, rawSettingsProviderName: FromAppSettingsProviderName)]
        public Setting<int> Port { get; private set; }

        // this setting will be retrieved by key TMP with provider named "env" 
        // but if its retrieval or conversion will be impossible, it will be just a default string value (null)
        [ToFill("TMP", isRequired: false, rawSettingsProviderName: FromEnvironmentProviderName)]
        public string TempFolderPath { get; private set; }
    }
}
