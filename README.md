# RapidSettings [![Build status](https://ci.appveyor.com/api/projects/status/1r2o5w4tsg11fatf/branch/master?svg=true)](https://ci.appveyor.com/project/baterja/rapidsettings/branch/master)
Simple and extensible way to make web.config/app.config/env vars/whatever easier to read and strongly typed!

TODOs before I can name it 1.0 version:
- [x] Test everything thoroughly,
- [ ] Learn how to use AppVeyor to properly patch version numbers,
- [x] Create some QuickStart or another tutorial,
- [x] Organize namespaces because it's too many of them to do so few things,

And after that maybe...:
- [ ] Create separate packages with more advanced Converters and Providers,
- [ ] Make more specific exceptions (if someone expresses such a need),
- [ ] Add another interface for Providers which will enable retrieving multiple raw values at once,
- [ ] Make conversion optional when type of setting retrieved by Provider is assignable to type of target property (+ setting to turn it on/off),
- [ ] Make retrieving raw setting values with Providers run in parallel if there are multiple Providers and they are async (+ setting to turn it on/off),

# Purpose
Ever used `ConfigurationManager.AppSettings["foo"]`, `Environment.GetEnvironmentVariable("bar");` or something like that? So you probably know that it can sometimes go out of control and spread through your project. Common method of keeping those settings tamed is creating some class and filling its properties in constructor like that:
```csharp
class SomeSettings
{
    public SomeSettings()
    {
        var host = ConfigurationManager.AppSettings[nameof(this.Host)];
        this.Host = new Uri(host);

        var port = ConfigurationManager.AppSettings[nameof(this.Port)];
        this.Port = int.Parse(port);

        this.TempFolderPath = Environment.GetEnvironmentVariable("TMP");
    }

    public Uri Host { get; }

    public int Port { get; }

    public string TempFolderPath { get; }
}
```
This project exists to make it easier, reusable and more flexible.

# Features
TODO

# Quickstart

Working project with below examples is in RapidSettings.Examples (it's net45).
The example shows basic scenario - there are some settings in app.config and another one as environment variable that should be retrieved and converted to some class properties.

Assuming that you have a section in app.config:
```xml
<appSettings>
    <add key="Host" value="http://nuget.org" />

    <!-- Try uncommenting a value below and run an example again. -->
    <add key="Port" value="1234" />
</appSettings>
```

create a class with some properties to fill (at least private setters are required):

```csharp
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
```

and the filling part:

```csharp
(using RapidSettings.Core;)

var converterChooser = new SettingsConverterChooser(new[] { new StringToFrameworkTypesConverter() });
var providersByNames = new Dictionary<string, IRawSettingsProvider> {
    { SomeSettings.FromEnvironmentProviderName, new FromEnvironmentProvider() },
    { SomeSettings.FromAppSettingsProviderName, new FromAppSettingsProvider() }
};
var settingsFiller = new SettingsFiller(converterChooser, providersByNames);

var settings = settingsFiller.CreateWithSettings<SomeSettings>();
```