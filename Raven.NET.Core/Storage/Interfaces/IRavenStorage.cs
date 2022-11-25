using System;
using System.Collections.Concurrent;
using Raven.NET.Core.Observers.Interfaces;

namespace Raven.NET.Core.Storage.Interfaces
{
    public interface IRavenStorage
    {
        public bool SubjectExists(Guid key);
        public string SubjectGet(Guid key);
        public bool SubjectTryAdd(Guid key, string value);
        public bool SubjectTryUpdate(Guid key, string value);
        public void SubjectRemove(Guid key);

        public bool SubjectTypeExists(Type type);
        public ConcurrentDictionary<string, string> SubjectTypeGet(Type type);
        public bool SubjectTypeTryAdd(Type type, ConcurrentDictionary<string, string> dictionary);
        public bool SubjectTypeTryUpdate(Type type, ConcurrentDictionary<string, string> newDictionary);
        public void SubjectTypeRemove(Type type);

        // Those methods require the base Type dictionary entry to exist
        public bool SubjectTypeValueExists(Type type, string innerKey);
        public string SubjectTypeValueGet(Type type, string innerKey);
        public bool SubjectTypeValueTryAdd(Type type, string innerKey, string innerValue);
        public bool SubjectTypeValueTryUpdate(Type type, string innerKey, string newValue);
        public void SubjectTypeValueRemove(Type type, string innerKey);

        public bool RavenWatcherExists(string key);
        public IRaven RavenWatcherGet(string key);
        public bool RavenWatcherTryAdd(string key, IRaven value);
        public bool RavenWatcherTryUpdate(string key, IRaven newValue);
        public void RavenWatcherRemove(string key);

        public bool RavenTypeWatcherExists(Type type);
        public IRavenTypeWatcher RavenTypeWatcherGet(Type type);
        public bool RavenTypeWatcherTryAdd(Type type, IRavenTypeWatcher value);
        public bool RavenTypeWatcherTryUpdate(Type type, IRavenTypeWatcher newValue);
        public void RavenTypeWatcherRemove(Type type);
    }
}