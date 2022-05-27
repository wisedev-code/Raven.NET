using Raven.NET.PoC.Subject;

namespace Raven.NET.PoC.Observer;

public class Observer : IObserver
{
    public void Update(ISubject subject)
    {
        Console.WriteLine($"ObserverReceive get update state: {subject.State}");
    }
}