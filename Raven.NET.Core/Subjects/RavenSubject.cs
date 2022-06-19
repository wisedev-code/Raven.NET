using System;
using System.Collections.Generic;
using System.Linq;
using Raven.NET.Core.Observers;
using Raven.NET.Core.Extensions;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Static;

namespace Raven.NET.Core.Subjects
{
    public class RavenSubject
    {
        internal List<IRaven> Observers = new();
        private Guid UniqueId { get; set; }

        protected RavenSubject()
        {
            UniqueId = Guid.NewGuid();
        }

        public void TryNotify()
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

        public void Attach(IRaven ravenWatcher)
        {
            var cacheKey = this.CreateCacheValue();
            RavenCache.SubjectCache.TryAdd(UniqueId, this.CreateCacheValue());
            Observers.Add(ravenWatcher);
        }

        public void Detach(IRavenWatcher ravenWatcher)
        {
            Observers.Remove(ravenWatcher);
            if (!Observers.Any()) ;
                RavenCache.SubjectCache.Remove(UniqueId, out var _);
        }
    }
}