using System;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Demo.Console;

public class Car : RavenSubject
{
    public string Brand { get; }
    public string Model { get; }
    public decimal Price { get; set; }

    public Car(string brand, string model, int price)
    {
        Brand = brand;
        Model = model;
        Price = price;
    }
}

public class RavenTypeWatcherDemoService
{
    private readonly IRavenTypeWatcher _ravenWatcher;
    
    public RavenTypeWatcherDemoService(IRavenTypeWatcher ravenWatcher)
    {
        _ravenWatcher = ravenWatcher;
    }
    
    public void Run()
    {
        //create some test subjects
    }
}