using System;
using System.Collections.Generic;
using System.Linq;
using Raven.NET.Core.Exceptions;
using Raven.NET.Core.Observers;
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
        void IRavenProvider.AddRaven(string ravenName, IRaven raven, Type subjectType = default)
        {
            if (raven is IRavenTypeWatcher)
            {
                var existWithType = !RavenCache.RavenTypeWatcherCache.TryAdd(subjectType, (IRavenTypeWatcher)raven);
                if (existWithType)
                {
                    throw new RavenForTypeAlreadyExistsException(subjectType);
                }
            }

            var existWithName = !RavenCache.RavenWatcherCache.TryAdd(ravenName, raven);
            if (existWithName)
            {
                throw new RavenAlreadyExistsException(ravenName);
            }
        }

        void IRavenProvider.UpdateSubjects(string ravenName, IEnumerable<RavenSubject> subjects, Type type)
        {
            if (type != default)
            {
                var ravenTypeWatcher = RavenCache.RavenTypeWatcherCache[type] as RavenTypeWatcher;
                ravenTypeWatcher._watchedSubjects = subjects.ToList();
                RavenCache.RavenTypeWatcherCache[type] = ravenTypeWatcher;
            }

            var ravenWatcher = RavenCache.RavenWatcherCache[ravenName] as RavenWatcher;
            ravenWatcher._watchedSubjects = subjects.ToList();
            RavenCache.RavenWatcherCache[ravenName] = ravenWatcher;
        }

        /// <inheritdoc/>
        public void RemoveRaven(string ravenName)
        {
            var notExist = !RavenCache.RavenWatcherCache.TryRemove(ravenName, out _);
            if (notExist)
            {
                throw new RavenDoesNotExistsException(ravenName);
            }
        }

        /// <inheritdoc/>
        public IRaven GetRaven(string ravenName, Type type = default)
        {
            if (type != default)
            {
                var typeExists = RavenCache.RavenTypeWatcherCache.TryGetValue(type, out var typeWatcher);
                if (!typeExists)
                {
                    throw new RavenDoesNotExistsException(ravenName);
                }
                
                return typeWatcher;
            }

            var exists = RavenCache.RavenWatcherCache.TryGetValue(ravenName, out var watcher);
            if (!exists)
            {
                throw new RavenDoesNotExistsException(ravenName);
            }
            
            return watcher;
        }

        /// <inheritdoc/>
        public bool RavenExist(string ravenName) => RavenCache.RavenWatcherCache.ContainsKey(ravenName);

        /// <inheritdoc/>
        void IRavenProvider.UpdateRavens(RavenSubject subject)
        {
            subject.Observers.ForEach(raven => raven.Update(subject));
        }
    }
}