using System;
using System.Collections.Generic;
using System.Linq;
using Raven.NET.Core.Extensions;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Storage;
using Raven.NET.Core.Storage.Interfaces;

namespace Raven.NET.Core.Subjects
{
    /// <summary>
    /// Base class that provides all methods needed to handle base of observer pattern
    /// </summary>
    public class RavenSubject
    {
        private readonly IRavenStorage _ravenStorage = RavenStorage.Instance;
        
        internal List<IRaven> Observers = new();
        internal Guid UniqueId { get; set; }

        protected RavenSubject()
        {
            UniqueId = Guid.NewGuid();
            var type = GetType();
            if (_ravenStorage.RavenTypeWatcherExists(type))
            {
                _ravenStorage.RavenTypeWatcherGet(type).AttachSubject(this);
            }
        }

        internal void Attach(IRaven ravenWatcher)
        {
            var cacheKey = this.CreateCacheValue();
            _ravenStorage.SubjectTryAdd(UniqueId, this.CreateCacheValue());
            Observers.Add(ravenWatcher);
        }

        internal void Detach(IRaven ravenWatcher)
        {
            Observers.Remove(ravenWatcher);
            if (!Observers.Any())
            {
                _ravenStorage.SubjectRemove(UniqueId);
            }
        }
        
        public void TryNotify()
        {
            var type = GetType();
            var typeWatcherExists = _ravenStorage.RavenTypeWatcherExists(type);

            if (!typeWatcherExists)
            {
                SingleWatcherProcessing();
                return;
            }
            
            TypeWatcherProcessing();
        }

        private void SingleWatcherProcessing()
        {
            if (_ravenStorage.SubjectExists(UniqueId))
            {
                var valueToCache = this.CreateCacheValue();

                if (_ravenStorage.SubjectTryUpdate(UniqueId, valueToCache))
                {
                    Observers.ForEach(raven => raven.Update(this));
                }
            }
        }

        private void TypeWatcherProcessing()
        {
            var type = GetType();

            var key = type.GetProperty(_ravenStorage.RavenTypeWatcherGet(type).KeyName).GetValue(this).ToString();
            var valueToStore = this.CreateCacheValue();

            if (!_ravenStorage.SubjectTypeValueExists(type, key))
            {
                _ravenStorage.SubjectTypeValueTryAdd(type, key, valueToStore);
                Observers.Add(_ravenStorage.RavenTypeWatcherGet(type));
                return;;
            }

            var existingValue = _ravenStorage.SubjectTypeValueGet(type, key);

            if (existingValue != valueToStore)
            {
                Observers.Add(_ravenStorage.RavenTypeWatcherGet(type));
                _ravenStorage.RavenTypeWatcherGet(type).UpdateNewestSubject(key, this);
                _ravenStorage.SubjectTypeValueTryUpdate(type, key, valueToStore);
                Observers.ForEach(raven => raven.Update(this));
            }
        }
    }
}