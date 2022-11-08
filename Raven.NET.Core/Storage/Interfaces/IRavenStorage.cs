using System;
using System.Collections.Concurrent;
using Raven.NET.Core.Observers.Interfaces;

namespace Raven.NET.Core.Storage.Interfaces
{
    public interface IRavenStorage
    {
        public ConcurrentDictionary<Guid, string> SubjectStorage { get; }
        
        public ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> SubjectTypeStorage { get; }

        public ConcurrentDictionary<string, IRaven> RavenWatcherStorage { get; }

        public ConcurrentDictionary<Type, IRavenTypeWatcher> RavenTypeWatcherStorage { get; }
    }
}