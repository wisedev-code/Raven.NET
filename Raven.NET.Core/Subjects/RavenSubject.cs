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
            if (_ravenStorage.RavenTypeWatcherStorage.ContainsKey(type))
            {
                _ravenStorage.RavenTypeWatcherStorage[type].AttachSubject(this);
            }
        }

        internal void Attach(IRaven ravenWatcher)
        {
            var cacheKey = this.CreateCacheValue();
            _ravenStorage.SubjectStorage.TryAdd(UniqueId, this.CreateCacheValue());
            Observers.Add(ravenWatcher);
        }

        internal void Detach(IRaven ravenWatcher)
        {
            Observers.Remove(ravenWatcher);
            if (!Observers.Any())
            {
                _ravenStorage.SubjectStorage.Remove(UniqueId, out var _);
            }
        }
        
        public void TryNotify()
        {
            var type = GetType();
            var typeWatcherExists = _ravenStorage.RavenTypeWatcherStorage.ContainsKey(type);

            if (!typeWatcherExists)
            {
                SingleWatcherProcessing();
                return;
            }
            
            TypeWatcherProcessing();
        }

        private void SingleWatcherProcessing()
        {
            if (_ravenStorage.SubjectStorage.ContainsKey(UniqueId))
            {
                var valueToCache = this.CreateCacheValue();
                if (_ravenStorage.SubjectStorage[UniqueId] != valueToCache)
                {
                    _ravenStorage.SubjectStorage[UniqueId] = valueToCache;
                    Observers.ForEach(raven => raven.Update(this));
                }
            }
        }

        private void TypeWatcherProcessing()
        {
            var type = GetType();

            var key = type.GetProperty(_ravenStorage.RavenTypeWatcherStorage[type].KeyName).GetValue(this).ToString();
            var valueToStoreInCache = this.CreateCacheValue();
            
            if (!_ravenStorage.SubjectTypeStorage[type].TryGetValue(key, out string cachedValue))
            {
                _ravenStorage.SubjectTypeStorage[type].TryAdd(key, valueToStoreInCache);
                Observers.Add(_ravenStorage.RavenTypeWatcherStorage[type]);
                return;
            }

            if (cachedValue != valueToStoreInCache)
            {
                Observers.Add(_ravenStorage.RavenTypeWatcherStorage[type]);
                _ravenStorage.RavenTypeWatcherStorage[type].UpdateNewestSubject(key, this);
                _ravenStorage.SubjectTypeStorage[type][key] = valueToStoreInCache;
                Observers.ForEach(raven => raven.Update(this));
            }
        }
    }
}