using System;
using Microsoft.Extensions.Logging;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Demo.Console;

public class Phone : RavenSubject
{
    public string Brand { get; }
    public string Model { get; }
    public decimal Price { get; set; }

    public Phone(string brand, string model, int price)
    {
        Brand = brand;
        Model = model;
        Price = price;
    }
}

public class RavenWatcherDemoService
{
    private readonly IRavenWatcher _ravenWatcher1;
    private readonly IRavenWatcher _ravenWatcher2;
    private readonly IRavenWatcher _ravenWatcher3;

    public RavenWatcherDemoService(
        IRavenWatcher ravenWatcher1,
        IRavenWatcher ravenWatcher2,
        IRavenWatcher ravenWatcher3)
    {
        _ravenWatcher1 = ravenWatcher1;
        _ravenWatcher2 = ravenWatcher2;
        _ravenWatcher3 = ravenWatcher3;
    }

    public void Run()
    {
        //create some test subjects
        var testPhone1 = new Phone("Sumsang", "Coment A5", 500);
        var testPhone2 = new Phone("Zaiomy", "Wedmi Note 8", 340);
        var testPhone3 = new Phone("Kokorola", "H7", 450);

        var watcher1 = _ravenWatcher1.Create("Raven1", PhoneUpdated, (options) => { options.AutoDestroy = true; });
        var watcher2 = _ravenWatcher2.Create("Raven2", PhoneUpdated, (options) => { options.AutoDestroy = true; });
        var watcher3 = _ravenWatcher3.Create("Raven3", PhoneUpdated, (options) => { options.AutoDestroy = true; });

        watcher1.Watch(testPhone1);
        watcher2.Watch(testPhone1);
        watcher3.Watch(testPhone1);

        testPhone1.Price = 550;
        testPhone1.TryNotify();

        //Will not do anything because its not watched
        testPhone2.Price = 400;
        testPhone2.TryNotify();

        watcher1.Watch(testPhone2).Watch(testPhone3);

        testPhone2.Price = 420;
        testPhone2.TryNotify();

        //will not do anything because its not changed
        testPhone3.TryNotify();

        watcher1.Stop("Raven1");

        testPhone1.Price = 200;
        testPhone2.Price = 400;
        testPhone3.Price = 800;
        testPhone1.TryNotify();
        testPhone2.TryNotify();
        testPhone3.TryNotify();

        System.Console.ReadLine();
    }

    private bool PhoneUpdated(RavenSubject ravenSubject)
    {
        var phone = ravenSubject as Phone;
        System.Console.WriteLine($"Phone {phone.Brand} - {phone.Model} has new price: {phone.Price}");
        return true;
    }
}