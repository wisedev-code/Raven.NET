# Raven.NET
## Installation
To make use of Raven.NET library and its most important features you need to install Core nuget package, you can do this with *dotnet* command like this:
> dotnet add package Raven.NET.Core

or via nuget package manager console:

> Install-Package Raven.NET.Core 

To check newest updates and additional information please check direct link:
https://www.nuget.org/packages/Raven.NET.Core
## Getting started
To use Raven.NET first step is to register necesary dependencies in service collection, its possible to do this manually (it will be described in details in docs) but there is already extension method prepared for this that you should call in your *ConfigureServices* method. Example with IHostedService below.

```
 var host = Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.ConfigureRaven(configuration);
        }).Build();
```

Then to make use of the most simple observer you should inject IRavenWatcher interface.
With this you can create new watchers by using _Create_ method. It needs 2 parameters, when first is name of watcher (needs to be unique) and second is callback where we will get updates. Additionally it can have third parameter which is options, if not provided it will load options from appsettings and if those does not exist then it will use default configuration. Example of usage is shown below:

```
var watcher1 = _ravenWatcher1.Create("Raven1", PhoneUpdated, (options) => { options.AutoDestroy = true; });
```

After our watcher is created then we can tell him to watch certain object.

```
watcher1.Watch(testPhone2).Watch(testPhone3);
```

With this code after we will make change to watched objects and notify observers about it we get message in our callback

```
testPhone2.Price = 420;
testPhone2.TryNotify();
```

And we get in our console logged:

![image](https://user-images.githubusercontent.com/111281468/187757222-ce7eebeb-6cd6-4a43-a5ce-2fb04d773ad3.png)

## Docs
<img src="https://cdn-icons-png.flaticon.com/512/5229/5229377.png" width="200" height="200" />


## Why the name raven?
<img align="right" height="400" width="300" src="https://user-images.githubusercontent.com/105814382/169652167-82a3570b-0c55-4498-b313-1a66eeec893f.png"/>
<p align="left" >Some time ago I watched a series in which a raven was an animal that was used for observation. It stared at the target 
carefully as if it wanted to see all the details and potential clues. For those interested, there were also vampires and werewolves 
in the series (probably many of you already guess what the series is about). I recently remembered this and read a bit about it, 
even found out that ravens are considered animals that can be tracked by military and governmental organizations. Hence the idea to name 
the library whose core will be based on the observer pattern as Raven. An additional advantage of this approach is easy understanding of the context and purpose, I would like this library to be able to describe what it does, and this approach is also evident in the interfaces and approach to using the framework. I believe that basing the elements of logic that is visible in the code on elements that are visible in the natural environment, the principles of operation should be easier to understand
 </p>

