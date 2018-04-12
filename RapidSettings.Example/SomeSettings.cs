using RapidSettings.Core;
using System;

namespace RapidSettings.Example
{
    class SomeSettings
    {
        public const string FromEnvironmentProviderName = "env";
        public const string FromAppSettingsProviderName = "config";

        [ToFill(rawSettingsProviderName: FromAppSettingsProviderName)]
        public Uri Host { get; private set; }

        [ToFill(isRequired: false, rawSettingsProviderName: FromAppSettingsProviderName)]
        public Setting<int> Port { get; private set; }

        [ToFill("TMP", isRequired: false, rawSettingsProviderName: FromEnvironmentProviderName)]
        public string TempFolderPath { get; private set; }
    }
}
