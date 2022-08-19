using System;
using Raven.NET.Core.Exceptions;
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
        public void AddRaven(string ravenName, IRaven raven, Type subjectType = default)
        {
            if (raven is IRavenTypeWatcher)
            {
                var existWithType = !RavenCache.RavenTypeWatcherCache.TryAdd(subjectType, (IRavenTypeWatcher)raven);
                if (!existWithType)
                {
                    throw new RavenForTypeAlreadyExistException(subjectType);
                }
            }

            var existWithName = !RavenCache.RavenWatcherCache.TryAdd(ravenName, raven);
            if (!existWithName)
            {
                throw new RavenAlreadyExistException(ravenName);
            }
        }

        /// <inheritdoc/>
        public void RemoveRaven(string ravenName)
        {
            var notExist = !RavenCache.RavenWatcherCache.TryRemove(ravenName, out _);
            if (notExist)
            {
                throw new RavenDoesNotExistException(ravenName);
            }
        }

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