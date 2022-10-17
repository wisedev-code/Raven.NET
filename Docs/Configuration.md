### Configuration

One of the most important thing of having Raven.NET in project is to properly configure it. We hope most of the configuration should be self-explanatory, but still it's good to describe some configuration parameters. It's also worth to mention that we use options monitor, so parameters can be changed in runtime. 

There are 3 ways to have your ravens configured. 
 - appsettings.json
 - overwrite configuration by parameter in create method
 - default configuration

****

**Configuration parameters:**
- AutoDestroy (by default false): It determines if raven should be removed if there are no subjects to watch.
- BreakOnUpdateException (by default false): It determines if raven will throw exception when there is error in update callback
- BackgroundWorker (for RavenWatcher false by default, for TypeWatcher true): It determines, if there will be additional thread to look for changes. For TypeWatcher it cannot be overwritten to false.
- BackgroundWorkerInterval (by default 1s): If previous property is set to true, then this decides how often check will be invoked.
- LogLevel (by default Warning): RavneWatcher is doing some internal logging, this is used to specify level of logged information.

**Example of configuration with appsettings.json**

```python
{
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        }
    },
    "Raven": {
        "RavenTypeTest": {
            "AutoDestroy": true,
            "BackgroundWorkerInterval": 2,
            "BreakOnUpdateException": true,
            "LogLevel": "Warning"
        }
    }
}

```