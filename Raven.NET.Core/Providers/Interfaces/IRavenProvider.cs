using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Providers.Interfaces
{
    public interface IRavenProvider
    {
        void AddRaven(string ravenName, IRaven raven);
        void RemoveRaven(string ravenName);
        void UpdateRavens(RavenSubject subject);
    }
}