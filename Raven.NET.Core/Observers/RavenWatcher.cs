using System;
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
    public class RavenWatcher : IRavenWatcher
    {
        private readonly IRavenProvider _ravenProvider;
        private readonly IRavenSettingsProvider _ravenSettingsProvider;

        private RavenSettings _ravenSettings = new();
        private List<RavenSubject> watchedSubjects = new List<RavenSubject>();
        private Func<RavenSubject,bool> updateAction;
        
        public RavenWatcher(IRavenProvider ravenProvider, IRavenSettingsProvider ravenSettingsProvider)
        {
            _ravenProvider = ravenProvider;
            _ravenSettingsProvider = ravenSettingsProvider;
        }
        
        /// <inheritdoc/>
        void IRaven.Update(RavenSubject subject)
        {
            updateAction(subject);
        }

        /// <inheritdoc/>
        public IRavenWatcher Create(string name, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null)
        {
            updateAction = callback;
            _ravenProvider.AddRaven(name,this);

            var ravenSettings = _ravenSettingsProvider.GetRaven(name);
            if (ravenSettings != null)
            {
                _ravenSettings = ravenSettings;
            }

            if (options != null)
            {
                options(_ravenSettings);
            }
            
            return this;
        }

        /// <inheritdoc/>
        public IRavenWatcher Watch(RavenSubject subject)
        {
            subject.Attach(this);
            watchedSubjects.Add(subject);
            return this;
        }

        /// <inheritdoc/>
        public void UnWatch(string name, RavenSubject subject)
        {
            var raven = _ravenProvider.GetRaven(name) as RavenWatcher;
            raven.watchedSubjects.Remove(subject);
            
            if(raven._ravenSettings.AutoDestroy)
                TryDestroy(raven, name);
        }
        
        /// <inheritdoc/>
        public void Stop(string name)
        {
            var raven = _ravenProvider.GetRaven(name) as RavenWatcher;
            raven.watchedSubjects.ForEach(x => x.Detach(this));
            
            if(raven._ravenSettings.AutoDestroy)
                TryDestroy(raven, name);
        }

        /// <inheritdoc/>
        private void TryDestroy(RavenWatcher ravenWatcher, string ravenName)
        {
            if (!ravenWatcher.watchedSubjects.Any())
            {
                RavenCache.RavenWatcherCache.Remove(ravenName, out _);
            }
        }
    }
}