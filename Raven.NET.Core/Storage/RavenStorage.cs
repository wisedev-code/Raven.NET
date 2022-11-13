using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        // private static RavenStorage _instance;
        // private static readonly object Lock = new();
        //
        // public static RavenStorage Instance
        // {
        //     get
        //     {
        //         lock (Lock)
        //         {
        //             _instance ??= new RavenStorage();
        //         }
        //         return _instance;
        //     }
        // }
        //
        // public static RavenStorage TestInstance
        // {
        //     get
        //     {
        //         _instance = new RavenStorage();
        //         return _instance;
        //     }
        // }

        private RavenStorage _instance;

        public RavenStorage GetInstance()
        {
            _instance ??= new RavenStorage();
            return _instance;
        }

        public bool SubjectExists(Guid key) => SubjectStorage.ContainsKey(key);
        public string SubjectGet(Guid key) => SubjectStorage[key];
        public bool SubjectTryAdd(Guid key, string value) => SubjectStorage.TryAdd(key, value);
        public bool SubjectTryUpdate(Guid key, string newValue) => SubjectStorage.TryUpdate(key, newValue, SubjectStorage[key]);
        public void SubjectRemove(Guid key) => SubjectStorage.Remove(key, out _);

        
        public bool SubjectTypeExists(Type type) => SubjectTypeStorage.ContainsKey(type);
        public ConcurrentDictionary<string, string> SubjectTypeGet(Type type) => SubjectTypeStorage[type];
        public bool SubjectTypeTryAdd(Type type, ConcurrentDictionary<string, string> dictionary) 
            => SubjectTypeStorage.TryAdd(type, dictionary);
        public bool SubjectTypeTryUpdate(Type type, ConcurrentDictionary<string, string> newDictionary)
            => SubjectTypeStorage.TryUpdate(type, newDictionary, SubjectTypeStorage[type]);
        public void SubjectTypeRemove(Type type) => SubjectTypeStorage.Remove(type, out _);
        
        
        public bool SubjectTypeValueExists(Type type, string innerKey) 
            => SubjectTypeStorage[type].ContainsKey(innerKey);
        public string SubjectTypeValueGet(Type type, string innerKey)
            => SubjectTypeStorage[type][innerKey];
        public bool SubjectTypeValueTryAdd(Type type, string innerKey, string innerValue)
            => SubjectTypeStorage[type].TryAdd(innerKey, innerValue);
        public bool SubjectTypeValueTryUpdate(Type type, string innerKey, string newValue)
            => SubjectTypeStorage[type].TryUpdate(innerKey, newValue, SubjectTypeStorage[type][innerKey]);
        public void SubjectTypeValueRemove(Type type, string innerKey)
            => SubjectTypeStorage[type].Remove(innerKey, out _);


        public bool RavenWatcherExists(string key) => RavenWatcherStorage.ContainsKey(key);
        public IRaven RavenWatcherGet(string key) => RavenWatcherStorage[key];
        public bool RavenWatcherTryAdd(string key, IRaven value) => RavenWatcherStorage.TryAdd(key, value);
        public bool RavenWatcherTryUpdate(string key, IRaven newValue) 
            => RavenWatcherStorage.TryUpdate(key, newValue, RavenWatcherStorage[key]);
        public void RavenWatcherRemove(string key) => RavenWatcherStorage.Remove(key, out _);

        
        public bool RavenTypeWatcherExists(Type type) => RavenTypeWatcherStorage.ContainsKey(type);
        public IRavenTypeWatcher RavenTypeWatcherGet(Type type) => RavenTypeWatcherStorage[type];
        public bool RavenTypeWatcherTryAdd(Type type, IRavenTypeWatcher value) => RavenTypeWatcherStorage.TryAdd(type, value);
        public bool RavenTypeWatcherTryUpdate(Type type, IRavenTypeWatcher newValue) 
            => RavenTypeWatcherStorage.TryUpdate(type, newValue, RavenTypeWatcherStorage[type]);
        public void RavenTypeWatcherRemove(Type type) => RavenTypeWatcherStorage.Remove(type, out _);
    }
}