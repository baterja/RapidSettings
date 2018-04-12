# RapidSettings [![Build status](https://ci.appveyor.com/api/projects/status/1r2o5w4tsg11fatf/branch/master?svg=true)](https://ci.appveyor.com/project/baterja/rapidsettings/branch/master)
Simple and extensible way to make web.config/app.config/env vars/whatever easier to read and strongly typed!

TODOs before I can name it 1.0 version:
- [x] Test everything thoroughly,
- [ ] Learn how to use AppVeyor to properly patch version numbers,
- [ ] Create some QuickStart or another tutorial,
- [x] Organize namespaces because it's too many of them to do so few things,

And after that maybe...:
- [ ] Create separate packages with more advanced Converters and Providers,
- [ ] Make more specific exceptions (if someone expresses such a need),
- [ ] Add another interface for Providers which will enable retrieving multiple raw values at once,
- [ ] Make conversion optional when type of setting retrieved by Provider is assignable to type of target property (+ setting to turn it on/off),
- [ ] Make retrieving raw setting values with Providers run in parallel if there are multiple Providers and they are async (+ setting to turn it on/off),