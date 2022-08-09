using System;
using System.Collections.Generic;
using System.Linq;
using Raven.NET.Core.Observers;
using Raven.NET.Core.Extensions;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Static;

namespace Raven.NET.Core.Subjects
{
    /// <summary>
    /// Base class that provides all methods needed to handle base of observer pattern
    /// </summary>
    public class RavenSubject
    {
        internal List<IRaven> Observers = new();
        internal Guid UniqueId { get; set; }

        protected RavenSubject()
        {
            UniqueId = Guid.NewGuid();
            var type = GetType();
            if (RavenCache.RavenTypeWatcherCache.ContainsKey(type))
            {
                RavenCache.RavenTypeWatcherCache[type].AttachSubject(this);
            }
        }

        internal void Attach(IRaven ravenWatcher)
        {
            var cacheKey = this.CreateCacheValue();
            RavenCache.SubjectCache.TryAdd(UniqueId, this.CreateCacheValue());
            Observers.Add(ravenWatcher);
        }

        internal void Detach(IRaven ravenWatcher)
        {
            Observers.Remove(ravenWatcher);
            if (!Observers.Any())
            {
                RavenCache.SubjectCache.Remove(UniqueId, out var _);
            }
        }
        
        public void TryNotify()
        {
            var type = GetType();
            var typeWatcherExists = RavenCache.RavenTypeWatcherCache.ContainsKey(type);

            if (!typeWatcherExists)
            {
                SingleWatcherProcessing();
                return;
            }
            
            TypeWatcherProcessing();
        }

        private void SingleWatcherProcessing()
        {
            if (RavenCache.SubjectCache.ContainsKey(UniqueId))
            {
                if (RavenCache.SubjectCache[UniqueId] != this.CreateCacheValue())
                {
                    RavenCache.SubjectCache[UniqueId] = this.CreateCacheValue();
                    Observers.ForEach(raven => raven.Update(this));
                }
            }
        }

        private void TypeWatcherProcessing()
        {
            var type = GetType();

            var key = type.GetProperty(RavenCache.RavenTypeWatcherCache[type].KeyName).GetValue(this).ToString();
            var valueToStoreInCache = this.CreateCacheValue();
            
            if (!RavenCache.SubjectTypeCache[type].TryGetValue(key, out string cachedValue))
            {
                RavenCache.SubjectTypeCache[type].TryAdd(key, valueToStoreInCache);
                Observers.Add(RavenCache.RavenTypeWatcherCache[type]);
                return;
            }

            if (cachedValue != valueToStoreInCache)
            {
                Observers.Add(RavenCache.RavenTypeWatcherCache[type]);
                RavenCache.RavenTypeWatcherCache[type].UpdateNewestSubject(key, this);
                RavenCache.SubjectTypeCache[type][key] = valueToStoreInCache;
                Observers.ForEach(raven => raven.Update(this));
            }
        }
    }
}