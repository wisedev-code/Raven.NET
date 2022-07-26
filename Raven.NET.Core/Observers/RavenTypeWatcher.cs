using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Static;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers
{
    /// <inheritdoc/>
    public class RavenTypeWatcher : IRavenTypeWatcher
    {
        private readonly IRavenProvider _ravenProvider;
        private readonly IRavenSettingsProvider _ravenSettingsProvider;

        private RavenSettings _ravenSettings = new();
        private List<RavenSubject> _watchedSubjects = new();
        private Func<RavenSubject,bool> _updateAction;
        private string _keyName;

        public RavenTypeWatcher(IRavenProvider ravenProvider, IRavenSettingsProvider ravenSettingsProvider)
        {
            _ravenProvider = ravenProvider;
            _ravenSettingsProvider = ravenSettingsProvider;

            _ravenSettings.BackgroundWorker = true;
            _ravenSettings.BackgroundWorkerInterval = 1.0f;
        }
        
        /// <inheritdoc/>
        void IRaven.Update(RavenSubject subject)
        {
            _updateAction.Invoke(subject);
        }

        /// <inheritdoc/>
        public IRavenTypeWatcher Create<T>(string name, string keyName, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null) 
        {
            _updateAction = callback;
            _ravenProvider.AddRaven(name,this, typeof(T));
            RavenCache.SubjectTypeCache.TryAdd(typeof(T), new ConcurrentDictionary<string, string>());
            _keyName = keyName;

            var ravenSettings = _ravenSettingsProvider.GetRaven(name);
            if (ravenSettings != null)
            {
                if (ravenSettings.BackgroundWorker == null)
                {
                    ravenSettings.BackgroundWorker = true;
                    ravenSettings.BackgroundWorkerInterval = 1.0f;
                }
                
                _ravenSettings = ravenSettings;
            }

            if (options != null)
            {
                options(_ravenSettings);
            }

            if (_ravenSettings.BackgroundWorker.Value)
            {
                new System.Threading.Timer((e) => { BackgroundWorkerRun(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_ravenSettings.BackgroundWorkerInterval.Value));
            }

            return this;
        }

        /// <inheritdoc/>
        public void Exclude(RavenSubject subject)
        {
            _watchedSubjects.Remove(subject);
        }
        
        /// <inheritdoc/>
        public void Stop(string name)
        {
            var raven = _ravenProvider.GetRaven(name) as RavenTypeWatcher;
            raven._watchedSubjects.ForEach(x => x.Detach(this));
            
            if(raven._ravenSettings.AutoDestroy)
                TryDestroy(raven, name);
        }

        /// <inheritdoc/>
        void IRavenTypeWatcher.UpdateNewestSubject(string key, RavenSubject subject)
        {
            _watchedSubjects.RemoveAll(x => x.GetType().GetProperty(_keyName).GetValue(x).ToString() == key);
            _watchedSubjects.Add(subject);
        }
        
        /// <inheritdoc/>
        void IRavenTypeWatcher.AttachSubject(RavenSubject subject)
        {
            _watchedSubjects.Add(subject);
        }

        /// <inheritdoc/>
        string IRavenTypeWatcher.KeyName => _keyName;
        
        private void BackgroundWorkerRun()
        {
            foreach (var subject in _watchedSubjects.ToList())
            {
                subject.TryNotify();
            }
        }
        
        private void TryDestroy(RavenTypeWatcher ravenWatcher, string ravenName)
        {
            if (!ravenWatcher._watchedSubjects.Any())
            {
                RavenCache.RavenWatcherCache.Remove(ravenName, out _);
            }
        }
    }
}