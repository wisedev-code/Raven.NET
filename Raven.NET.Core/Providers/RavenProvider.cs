using System;
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
        public bool AddRaven(string ravenName, IRaven raven, Type subjectType = default)
        {
            if (raven is IRavenTypeWatcher)
            {
                return RavenCache.RavenTypeWatcherCache.TryAdd(subjectType, (IRavenTypeWatcher)raven);
            }

            return RavenCache.RavenWatcherCache.TryAdd(ravenName, raven);
        }

        /// <inheritdoc/>
        public bool RemoveRaven(string ravenName) => RavenCache.RavenWatcherCache.TryRemove(ravenName, out _);

        /// <inheritdoc/>
        public IRaven GetRaven(string ravenName, Type type = default)
        {
            if (type != default)
            {
                RavenCache.RavenTypeWatcherCache.TryGetValue(type, out var typeWatcher);
                return typeWatcher;
            }

            RavenCache.RavenWatcherCache.TryGetValue(ravenName, out var watcher);
            return watcher;
        }

        /// <inheritdoc/>
        public bool RavenExist(string ravenName) => RavenCache.RavenWatcherCache.ContainsKey(ravenName);

        /// <inheritdoc/>
        public void UpdateRavens(RavenSubject subject)
        {
            subject.Observers.ForEach(raven => raven.Update(subject));
        }
    }
}