using Raven.NET.PoC.Subject;

namespace Raven.NET.PoC.Observer;

public interface IObserver
{
    //Receives update from subjects
    void Update(ISubject subject);
}