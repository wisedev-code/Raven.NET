using System;
using System.Text.Json.Serialization;
using System.Threading;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Demo.Console;

public class Car : RavenSubject
{
    public int Id { get; }
    public string Brand { get; }
    public string Model { get; }
    public decimal Price { get; set; }

    public Car(int id, string brand, string model, int price)
    {
        Id = id;
        Brand = brand;
        Model = model;
        Price = price;
    }
}

public class RavenTypeWatcherDemoService
{
    private readonly IRavenTypeWatcher _ravenTypeWatcher;
    private readonly IRavenProvider _ravenProvider;

    public RavenTypeWatcherDemoService(IRavenTypeWatcher ravenWatcher, IRavenProvider ravenProvider)
    {
        _ravenTypeWatcher = ravenWatcher;
        _ravenProvider = ravenProvider;
    }
    
    public void Run()
    {
        _ravenTypeWatcher.Create<Car>("RavenTypeWatcherExample", nameof(Console.Car.Id), Callback);

        var ravenCarWatcher = _ravenProvider.GetRaven("RavenTypeWatcherExample", typeof(Car)); // will return created raven

        var isCarRavenRegistered = _ravenProvider.RavenExist("RavenTypeWatcherExample"); // will return true
        
         _ravenProvider.RemoveRaven("RavenTypeWatcherExample");
         
        var isCarRavenRegisteredAfter = _ravenProvider.RavenExist("RavenTypeWatcherExample"); // will return false

        var Car = new Car(1231, "Audi", "A2", 40000);
        Thread.Sleep(3000);
        
        var Car2 = new Car(1231, "Audi", "A2", 43000);
        Thread.Sleep(3000);

        var Car3 = new Car(1231, "Audi", "A2", 43000);
        Thread.Sleep(3000);

        var Car4 = new Car(1231, "Audi", "A2", 47000);
        
        while(true){Thread.Sleep(3000);} //keep program running
    }

    private bool Callback(RavenSubject arg)
    {
        System.Console.WriteLine($"Car changed: {(arg as Car).Brand}|{(arg as Car).Model}, new price: {(arg as Car).Price}");
        return true;
    }
}