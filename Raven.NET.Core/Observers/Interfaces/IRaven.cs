using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers.Interfaces
{
    public interface IRaven
    {
        internal void Update(RavenSubject subject);
    }
}