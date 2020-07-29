# RapidSettings [![Build status](https://ci.appveyor.com/api/projects/status/1r2o5w4tsg11fatf/branch/master?svg=true)](https://ci.appveyor.com/project/baterja/rapidsettings/branch/master) [![NuGet](https://img.shields.io/nuget/vpre/RapidSettings.svg)](https://www.nuget.org/packages/RapidSettings/)
Simple and extensible way to make web.config/app.config/env vars/whatever easier to read and strongly typed!

TODOs:
- [ ] Make conversion optional when type of setting retrieved by Provider is assignable to type of target property (+ setting to turn it on/off),
- [ ] Add Converters chaining to ```ISettingsConverterChooser``` (if there is converter which converts from ```A``` to ```B``` and the other one converts from ```B``` to ```C``` then conversion from ```A``` to ```C``` should be possible)

# Table of contents
1. [Purpose](#Purpose)
2. [Quickstart](#Quickstart)
3. [Features](#Features)
   1. [Decorating properties with ```ToFillAttribute```](#ToFillAttribute)
   2. [Retrieval](#Retrieval)
   3. [Choosing converter](#ChoosingConverter)
   4. [Conversion](#Conversion)

<a name="Purpose"></a>
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

<a name="Quickstart"></a>
# Quickstart

Working project with below examples is in RapidSettings.Examples (it's net45).
The example shows basic scenario - there are some settings in app.config and another one as environment variable that should be retrieved and converted to some class properties.

Assuming that you have a section in app.config:
```xml
<appSettings>
    <add key="Host" value="http://nuget.org" />
    <add key="Port" value="1234" />
</appSettings>
```

create a class with some properties to fill (at least private setters are required):

```csharp
class SomeSettings
{
    // this setting will be retrieved by key Host (default) with default provider
    // and if its retrieval or conversion will be impossible, exception will be thrown
    [ToFill]
    public Uri Host { get; private set; }

    // this setting will be retrieved by key Port (default) with default provider
    // but if its retrieval or conversion will be impossible, it will be just a default int value (0)
    [ToFill(isRequired: false)]
    public int Port { get; private set; }

    // this setting will be retrieved by key TMP with provider named ENV 
    // (which is the default key of FromEnvironmentProvider taken from SettingsFillerStaticDefaults)
    // but if its retrieval or conversion will be impossible, it will be just a default string value (null)
    [ToFill("TMP", isRequired: false, rawSettingsProviderName: SettingsFillerStaticDefaults.FromEnvironmentProviderKey)]
    public string TempFolderPath { get; private set; }
}
```

and the filling part:

```csharp
(using RapidSettings.Core;)

var settingsFiller = new SettingsFiller();

var settings = settingsFiller.CreateWithSettings<SomeSettings>();
```

<a name="Features"></a>
# Features
Those are the steps which are usally performed by this library:
- you prepare Providers, Converters, ConverterChooser and SettingsFiller and use it on some class with properties decorated with ```ToFill``` attribute,
- properties decorated with ```ToFill``` attribute are collected and for each of them:
  - setting is retrieved by ```string``` key by ```IRawSettingsProvider``` which has been chosen by attribute's property of is default,
  - target type of conversion is determined (unwrapping ```Nullable<>```),
  - converter is chosen by and used by ```SettingsConverterChooser```,
  - result of conversion is assigned to target property.

And this is how you can adjust specific steps to your needs:

<a name="ToFillAttribute"></a>
### Decorating properties with ```ToFillAttribute```
There you can set 3 things:
- ```key``` by which raw value of setting will be retrieved. By default it's the name of decorated property, 
- ```isRequired``` - if ```true``` exception will be thrown if retrieval/conversion of the setting fails. If setting is not required, it will then be ```default``` for its type,
- ```rawSettingsProviderName``` - there you choose which provider should be used to retrieve raw value of property. If null, default provider will be used.

<a name="Retrieval"></a>
### Retrieval
Settings in "raw" form are provided by ```IRawSettingsProvider``` with method ```GetRawSetting``` taking ```string``` key as parameter and returning some ```object```. Implementations of those interfaces provided by library are:
- ```FromIConfigurationProvider``` which needs to be created with ```IConfiguration``` instance to use - that's probably the one you need for .NET Core,
- ```FromAppSettingsProvider``` which uses ```ConfigurationManager.AppSettings.Get()```,
- ```FromEnvironmentProvider``` which uses ```Environment.GetEnvironmentVariable()```,
- ```FromFuncProvider``` - parameterizable provider which is just using ```Invoke()``` on ```Func<string, object>``` which it get in constructor,

And of course you can add your own by implementing ```IRawSettingsProvider```.

<a name="ChoosingConverter"></a>
### Choosing converter
```ISettingsConverterChooser``` is responsible by choosing which converter to use given source and target type of conversion. Default implementation ```SettingsConverterChooser``` is just selecting first Converter which ```CanConvert``` from source to target type. If you need more advanced way of choosing Converters you can implement ```ISettingsConverterChooser``` by yourself.

<a name="Conversion"></a>
### Conversion
You create converters (```IRawSettingsConverter```s) which will be used to convert settings from something to desired type. Currently there is one provided with RapidSettings - ```StringToFrameworkTypesConverter``` which handles conversion from ```string``` to following types:
- all framework's numeric types (```int```, ```decimal```, ```double```...)
- ```string```
- ```Guid```
- ```bool```
- ```DateTime```
- ```DateTimeOffset```
- ```Uri```

You can add new converters by implementing ```IRawSettingsConverter```. Or just inherit ```StringToFrameworkTypesConverter``` and adjust it to your needs. There's ```RawSettingsConverterBase``` abstract class which you can inherit if you want your own implementation to be easier. Beside of implementing ```IRawSettingsConverter``` it provides ```AddSupportForTypes()``` method which allow simple adding converting functions to your converter. It also handles variance of types.