
using Raven.NET.Core.Subjects;
using Raven.NET.PoC.Observer;
using Raven.NET.PoC.Subject;

Console.WriteLine("Hello, World!");

RavenTestSubject ravenTest = new RavenTestSubject();

ravenTest.State = 12;
ravenTest.TryNotify();

var subject = new Subject();
var observerA = new Observer();
subject.Attach(observerA);

var observerB = new Observer();
subject.Attach(observerB);

subject.State = 5;
subject.Notify();
Thread.Sleep(2000);

subject.State = 6;
subject.Notify();
Thread.Sleep(5000);

subject.Detach(observerB);

subject.State = 1;
subject.Notify();