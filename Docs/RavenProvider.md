- [RavenProvider](#ravenprovider)
  - [Available methods:](#available-methods)
  - [Usage example](#usage-example)

## RavenProvider

This interface is mainly used to work over RavenWatcher and RavenTypeWatcher. Core of its functionality is locked in internal methods and used within methods of mentioned interfaces. There are as well some method that can be used for end user, and it will be described below. Methods mainly used for access to internal RavenCache.

### Available methods:

`public void RemoveRaven(string ravenName)` - Method to definetly remove Raven from cache. In watcher interface stop method will only clear watcher list if not configured, raven will not be removed.

Params:
 - string ravenName: name of watcher you want to remove.

`public IRaven GetRaven(string ravenName, Type type = default)` - Method to get raven of given name from internal cache. By default, you will be able to get only standard raven watchers, but if you provide type parameter, then you get type watchers as well.

Params: 

 - string ravenName: name of watcher you want to get
 - Type type: needs to be provided if you seek for type watcher.

`public bool RavenExist(string ravenName)` - Method to check if raven is already created. Basically a nice wrapper over get method, if you only need information about existance.

Params: 
- string ravenName: name of raven you will check


****
### Usage example

```c#
  _ravenTypeWatcher.Create<Car>("RavenTypeWatcherExample", nameof(Console.Car.Id), Callback);

        var ravenCarWatcher = _ravenProvider.GetRaven("RavenTypeWatcherExample", typeof(Car)); // will return created raven

        var isCarRavenRegistered = _ravenProvider.RavenExist("RavenTypeWatcherExample"); // will return true
        
         _ravenProvider.RemoveRaven("RavenTypeWatcherExample");
         
        var isCarRavenRegisteredAfter = _ravenProvider.RavenExist("RavenTypeWatcherExample"); // will return false

```

