using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Raven.NET.Core.Observers;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Storage.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Storage
{
    public class RavenStorage : IRavenStorage
    {
        private ConcurrentDictionary<Guid, string> SubjectStorage { get; set; } = new();
        private ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> SubjectTypeStorage { get; set; } = new();
        private ConcurrentDictionary<string, IRaven> RavenWatcherStorage { get; set; } = new();
        private ConcurrentDictionary<Type, IRavenTypeWatcher> RavenTypeWatcherStorage { get; set; } = new();
        
        private static RavenStorage _instance = new();

        public static RavenStorage GetInstance()
        {
            _instance ??= new RavenStorage();
            return _instance;
        }

        public bool SubjectExists(Guid key) => _instance.SubjectStorage.ContainsKey(key);
        public string SubjectGet(Guid key) => _instance.SubjectStorage[key];
        public List<RavenSubject> GetAllSubjects()
        {
            var watchers = RavenWatcherStorage.Values.ToList();
            watchers.AddRange(RavenTypeWatcherStorage.Values);

            List<RavenSubject> subjects = new List<RavenSubject>();
            foreach (var watcher in watchers)
            {
                if (watcher is RavenTypeWatcher typeWatcher)
                {
                    foreach (var subject in typeWatcher._watchedSubjects)
                    {
                        var type = GetType();
                        subject.Key = type.GetProperty(((IRavenTypeWatcher)typeWatcher).KeyName)?.GetValue(this).ToString();
                        subjects.Add(subject);
                    }
                    
                    continue;
                }

                foreach (var subject in (watcher as RavenWatcher)._watchedSubjects)
                {
                    subject.Key = subject.UniqueId.ToString();
                    subjects.Add(subject);
                }
            }

            return subjects;
        }

        public bool SubjectTryAdd(Guid key, string value) => _instance.SubjectStorage.TryAdd(key, value);
        public bool SubjectTryUpdate(Guid key, string newValue) 
            => _instance.SubjectStorage.TryUpdate(key, newValue, _instance.SubjectStorage[key]);
        public void SubjectRemove(Guid key) => _instance.SubjectStorage.Remove(key, out _);

        
        public bool SubjectTypeExists(Type type) => _instance.SubjectTypeStorage.ContainsKey(type);
        public ConcurrentDictionary<string, string> SubjectTypeGet(Type type) => _instance.SubjectTypeStorage[type];

        public bool SubjectTypeTryAdd(Type type, ConcurrentDictionary<string, string> dictionary)
            => _instance.SubjectTypeStorage.TryAdd(type, dictionary);

        public bool SubjectTypeTryUpdate(Type type, ConcurrentDictionary<string, string> newDictionary)
            => _instance.SubjectTypeStorage.TryUpdate(type, newDictionary, _instance.SubjectTypeStorage[type]);
        public void SubjectTypeRemove(Type type) => _instance.SubjectTypeStorage.Remove(type, out _);
        
        
        public bool SubjectTypeValueExists(Type type, string innerKey) 
            => _instance.SubjectTypeStorage[type].ContainsKey(innerKey);
        public string SubjectTypeValueGet(Type type, string innerKey)
            => _instance.SubjectTypeStorage[type][innerKey];
        public bool SubjectTypeValueTryAdd(Type type, string innerKey, string innerValue)
            => _instance.SubjectTypeStorage[type].TryAdd(innerKey, innerValue);
        public bool SubjectTypeValueTryUpdate(Type type, string innerKey, string newValue)
            => _instance.SubjectTypeStorage[type].TryUpdate(innerKey, newValue, _instance.SubjectTypeStorage[type][innerKey]);
        public void SubjectTypeValueRemove(Type type, string innerKey)
            => _instance.SubjectTypeStorage[type].Remove(innerKey, out _);


        public bool RavenWatcherExists(string key) => _instance.RavenWatcherStorage.ContainsKey(key);
        public IRaven RavenWatcherGet(string key) => _instance.RavenWatcherStorage[key];
        public bool RavenWatcherTryAdd(string key, IRaven value) => _instance.RavenWatcherStorage.TryAdd(key, value);
        public bool RavenWatcherTryUpdate(string key, IRaven newValue) 
            => RavenWatcherStorage.TryUpdate(key, newValue, _instance.RavenWatcherStorage[key]);
        public void RavenWatcherRemove(string key) => _instance.RavenWatcherStorage.Remove(key, out _);
        public List<IRaven> GetAllRavens()
        {
            var watchers = RavenWatcherStorage.Values.ToList();
            watchers.AddRange(RavenTypeWatcherStorage.Values);

            return watchers;
        }


        public bool RavenTypeWatcherExists(Type type) => _instance.RavenTypeWatcherStorage.ContainsKey(type);
        public IRavenTypeWatcher RavenTypeWatcherGet(Type type) => _instance.RavenTypeWatcherStorage[type];
        public bool RavenTypeWatcherTryAdd(Type type, IRavenTypeWatcher value) => _instance.RavenTypeWatcherStorage.TryAdd(type, value);
        public bool RavenTypeWatcherTryUpdate(Type type, IRavenTypeWatcher newValue) 
            => _instance.RavenTypeWatcherStorage.TryUpdate(type, newValue, _instance.RavenTypeWatcherStorage[type]);
        public void RavenTypeWatcherRemove(Type type) => _instance.RavenTypeWatcherStorage.Remove(type, out _);
    }
}