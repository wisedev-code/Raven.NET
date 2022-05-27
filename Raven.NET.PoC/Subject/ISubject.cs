using Raven.NET.PoC.Observer;

namespace Raven.NET.PoC.Subject;

public interface ISubject
{
    int State { get; }
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
}