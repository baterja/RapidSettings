# RapidSettings [![Build status](https://ci.appveyor.com/api/projects/status/1r2o5w4tsg11fatf/branch/master?svg=true)](https://ci.appveyor.com/project/baterja/rapidsettings/branch/master) [![NuGet](https://img.shields.io/nuget/vpre/RapidSettings.svg)](https://www.nuget.org/packages/RapidSettings/)
Simple and extensible way to make appsettings.json/web.config/app.config/env vars/whatever easier to read and strongly typed!

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

Example projects are RapidSettings.Example (net48) and RapidSettings.Example.NetCore (netcoreapp3.1).

## .Net Framework 4.8
The example shows basic scenario - there are some settings in app.config and another one as an environment variable that should be retrieved and converted to some class properties.

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

## .Net Core 3.x (should also work for 1.x and 2.x with minor changes)
The example shows basic scenario - there are some settings in appsettings.json and another one as an environment variable that should be retrieved and converted to some class properties.

Assuming that you have appsettings.json:
```json
{
  "Host": "http://nuget.org",
  "Port": 1234,
}
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

// you'll probably get it from whatever DI container you use
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var settingsFiller = new SettingsFiller(defaultRawSettingsProvider: new FromIConfigurationProvider(configuration));

var settings = settingsFiller.CreateWithSettings<SomeSettings>();
```

<a name="Features"></a>
# Features
Those are the steps which are usally performed by this library:
- you prepare Providers, Converters, ConverterChooser and SettingsFiller and use it on some class with properties decorated with ```ToFill``` attribute,
- properties decorated with ```ToFill``` attribute are collected and for each of them:
  - setting is retrieved by ```string``` key by ```IRawSettingsProvider``` which has been chosen by attribute's property of is default,
  - target type of conversion is determined,
  - converter is chosen by and used by ```SettingsConverterChooser``` (or conversion is skipped if retrieved value can be directly assigned to target property),
  - converter does its job (possibly delegating work to another converters, for example for types wrapped in `Nullable<>`, `IEnumerable<>` or `KeyValuePair<>`),
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
- ```FromIConfigurationProvider``` which needs to be created with ```IConfiguration``` instance to use - that's probably the one you need for .NET Core. And the only one which can give you a `List<>` or `Dictionary<,>` from appsettings.json out of the box,
- ```FromAppSettingsProvider``` which uses ```ConfigurationManager.AppSettings.Get()```,
- ```FromEnvironmentProvider``` which uses ```Environment.GetEnvironmentVariable()```,
- ```FromFuncProvider``` - parameterizable provider which is just using ```Invoke()``` on ```Func<string, object>``` which it get in constructor,

And of course you can add your own by implementing ```IRawSettingsProvider```.

<a name="ChoosingConverter"></a>
### Choosing converter
```ISettingsConverterChooser``` is responsible by choosing which converter to use given source and target type of conversion. Default implementation ```SettingsConverterChooser``` is just selecting first Converter which ```CanConvert``` from source to target type. If you need more advanced way of choosing Converters you can implement ```ISettingsConverterChooser``` yourself.

<a name="Conversion"></a>
### Conversion
You create converters (```IRawSettingsConverter```s) which will be used to convert settings from something to desired type. Currently there are few provided with RapidSettings:
- ```StringToFrameworkTypesConverter``` which handles conversion from ```string``` to following types:
    - all framework's numeric types (```int```, ```decimal```, ```double```...)
    - ```string```
    - ```Guid```
    - ```bool```
    - ```DateTime```
    - ```DateTimeOffset```
    - ```Uri```

and below are "passthrough" converters which only unpack non-trivial types and pass them back to `ISettingsConverterChooser` for further conversion:
- ```NullableConverter``` handles ```Nullable``` structs unpacking,
- ```KeyValuePairConverter``` handles conversion of ```Key```s and ```Values``` of retrieved KVPs to target ```KeyValuePair``` type,
- ```EnumerableConverter``` - unpacks `IEnumerable`s and able to create `List<>`, `HashSet<>` or `Dictionary<,>`

You can add new converters by implementing ```IRawSettingsConverter```. Or just inherit ```StringToFrameworkTypesConverter``` and adjust it to your needs. There's ```RawSettingsConverterBase``` abstract class which you can inherit if you want your own implementation to be easier. Beside of implementing ```IRawSettingsConverter``` it provides ```AddSupportForTypes()``` method which allow simple adding converting functions to your converter. It also handles variance of types.