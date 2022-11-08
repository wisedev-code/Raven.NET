using System;
using System.Collections.Concurrent;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Storage.Interfaces;

namespace Raven.NET.Core.Storage
{
    public class RavenStorage : IRavenStorage
    {
        public ConcurrentDictionary<Guid, string> SubjectStorage { get; } = new();
        public ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> SubjectTypeStorage { get; } = new();
        public ConcurrentDictionary<string, IRaven> RavenWatcherStorage { get; } = new();
        public ConcurrentDictionary<Type, IRavenTypeWatcher> RavenTypeWatcherStorage { get; } = new();
        

        private static RavenStorage _instance;
        private static readonly object Lock = new();
        public static RavenStorage Instance
        {
            get
            {
                lock (Lock)
                {
                    _instance ??= new RavenStorage();
                }
                return _instance;
            }
        }
    }
}