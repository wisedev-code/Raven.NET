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
                var existWithType = !_ravenStorage.RavenTypeWatcherTryAdd(subjectType, (IRavenTypeWatcher)raven);
                if (existWithType)
                {
                    throw new RavenForTypeAlreadyExistsException(subjectType);
                }
            }
            
            var existWithName = !_ravenStorage.RavenWatcherTryAdd(ravenName, raven);
            if (existWithName)
            {
                throw new RavenAlreadyExistsException(ravenName);
            }
        }

        void IRavenProvider.UpdateSubjects(string ravenName, IEnumerable<RavenSubject> subjects, Type type)
        {
            if (type != default)
            {
                var ravenTypeWatcher = _ravenStorage.RavenTypeWatcherGet(type) as RavenTypeWatcher;
                ravenTypeWatcher._watchedSubjects = subjects.ToList();
                _ravenStorage.RavenTypeWatcherTryUpdate(type, ravenTypeWatcher);
            }

            var ravenWatcher = _ravenStorage.RavenWatcherGet(ravenName) as RavenWatcher;
            ravenWatcher._watchedSubjects = subjects.ToList();
            _ravenStorage.RavenWatcherTryUpdate(ravenName, ravenWatcher);
        }

        /// <inheritdoc/>
        public void RemoveRaven(string ravenName)
        {
            if (!_ravenStorage.RavenWatcherExists(ravenName))
            {
                throw new RavenDoesNotExistsException(ravenName);
            }
            
            _ravenStorage.RavenWatcherRemove(ravenName);;
        }

        /// <inheritdoc/>
        public IRaven GetRaven(string ravenName, Type type = default)
        {
            if (type != default)
            {
                if (!_ravenStorage.RavenTypeWatcherExists(type))
                {
                    throw new RavenDoesNotExistsException(ravenName);
                }

                return _ravenStorage.RavenTypeWatcherGet(type);
            }

            if (!_ravenStorage.RavenWatcherExists(ravenName))
            {
                throw new RavenDoesNotExistsException(ravenName);
            }

            return _ravenStorage.RavenWatcherGet(ravenName);
        }

        /// <inheritdoc/>
        public bool RavenExist(string ravenName) => _ravenStorage.RavenWatcherExists(ravenName);

        /// <inheritdoc/>
        void IRavenProvider.UpdateRavens(RavenSubject subject)
        {
            subject.Observers.ForEach(raven => raven.Update(subject));
        }
    }
}