using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers
{
    public class RavenTypeWatcher : IRavenTypeWatcher
    {
        void IRaven.Update(RavenSubject subject)
        {
            throw new System.NotImplementedException();
        }
    }
}