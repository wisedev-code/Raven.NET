using System;
using System.Collections.Generic;
using System.Linq;
using Raven.NET.Core.Exceptions;
using Raven.NET.Core.Observers;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Storage;
using Raven.NET.Core.Storage.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Providers
{
    /// <inheritdoc/>
    public class RavenProvider : IRavenProvider
    {
        private IRavenStorage _ravenStorage = RavenStorage.Instance;

        /// <inheritdoc/>
        void IRavenProvider.AddRaven(string ravenName, IRaven raven, Type subjectType = default)
        {
            if (raven is IRavenTypeWatcher)
            {
                var existWithType = !_ravenStorage.RavenTypeWatcherStorage.TryAdd(subjectType, (IRavenTypeWatcher)raven);
                if (existWithType)
                {
                    throw new RavenForTypeAlreadyExistsException(subjectType);
                }
            }

            var existWithName = !_ravenStorage.RavenWatcherStorage.TryAdd(ravenName, raven);
            if (existWithName)
            {
                throw new RavenAlreadyExistsException(ravenName);
            }
        }

        void IRavenProvider.UpdateSubjects(string ravenName, IEnumerable<RavenSubject> subjects, Type type)
        {
            if (type != default)
            {
                var ravenTypeWatcher = _ravenStorage.RavenTypeWatcherStorage[type] as RavenTypeWatcher;
                ravenTypeWatcher._watchedSubjects = subjects.ToList();
                _ravenStorage.RavenTypeWatcherStorage[type] = ravenTypeWatcher;
            }

            var ravenWatcher = _ravenStorage.RavenWatcherStorage[ravenName] as RavenWatcher;
            ravenWatcher._watchedSubjects = subjects.ToList();
            _ravenStorage.RavenWatcherStorage[ravenName] = ravenWatcher;
        }

        /// <inheritdoc/>
        public void RemoveRaven(string ravenName)
        {
            var notExist = !_ravenStorage.RavenWatcherStorage.TryRemove(ravenName, out _);
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
                var typeExists = _ravenStorage.RavenTypeWatcherStorage.TryGetValue(type, out var typeWatcher);
                if (!typeExists)
                {
                    throw new RavenDoesNotExistsException(ravenName);
                }
                
                return typeWatcher;
            }

            var exists = _ravenStorage.RavenWatcherStorage.TryGetValue(ravenName, out var watcher);
            if (!exists)
            {
                throw new RavenDoesNotExistsException(ravenName);
            }
            
            return watcher;
        }

        /// <inheritdoc/>
        public bool RavenExist(string ravenName) => _ravenStorage.RavenWatcherStorage.ContainsKey(ravenName);

        /// <inheritdoc/>
        void IRavenProvider.UpdateRavens(RavenSubject subject)
        {
            subject.Observers.ForEach(raven => raven.Update(subject));
        }
    }
}