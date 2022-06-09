using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Static;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Providers
{
    /// <inheritdoc/>
    public class RavenProvider : IRavenProvider
    {
        /// <inheritdoc/>
        public bool AddRaven(string ravenName, IRaven raven) => RavenStore.RavenWatcherStore.TryAdd(ravenName, raven);

        /// <inheritdoc/>
        public bool RemoveRaven(string ravenName) => RavenStore.RavenWatcherStore.TryRemove(ravenName, out _);

        /// <inheritdoc/>
        public bool RavenExist(string ravenName) => RavenStore.RavenWatcherStore.ContainsKey(ravenName);

        /// <inheritdoc/>
        public void UpdateRavens(RavenSubject subject)
        {
            subject.Observers.ForEach(raven => raven.Update(subject));
        }
    }
}