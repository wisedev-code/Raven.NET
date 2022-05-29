using Raven.NET.PoC.Observer;

namespace Raven.NET.PoC.Subject;

public class Subject : ISubject
{
    public int State { get; set; } = 0;
    
    private IList<IObserver> _observers = new List<IObserver>();
    
    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
        Console.WriteLine("Observer attached");
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine("Observer detached");
    }

    public void Notify()
    {
        Console.WriteLine("Notify all observers");
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }
}