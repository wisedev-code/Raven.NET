using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Static;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Providers
{
    public class RavenProvider : IRavenProvider
    {
        public void AddRaven(string ravenName, IRaven raven)
        {
            RavenStore.RavenWatcherStore.TryAdd(ravenName, raven);
        }

        public void RemoveRaven(string ravenName)
        {
            RavenStore.RavenWatcherStore.TryRemove(ravenName, out _);
        }

        public void UpdateRavens(RavenSubject subject)
        {
            subject.Observers.ForEach(raven => raven.Update(subject));
        }
    }
}