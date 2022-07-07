using System;
using System.Collections.Generic;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers
{
    /// <inheritdoc/>
    public class RavenTypeWatcher : IRavenTypeWatcher
    {
        private readonly IRavenProvider _ravenProvider;
        private readonly IRavenSettingsProvider _ravenSettingsProvider;

        private RavenSettings _ravenSettings = new();
        private List<RavenSubject> _watchedSubjects = new List<RavenSubject>();
        private Func<RavenSubject,bool> _updateAction;
        
        /// <inheritdoc/>
        void IRaven.Update(RavenSubject subject)
        {
            _updateAction.Invoke(subject);
        }

        /// <inheritdoc/>
        public IRavenTypeWatcher Create<T>(string name, string keyName, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null) 
        {
            _updateAction = callback;
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
        public void Exclude(string name, RavenSubject subject)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Stop(string name)
        {
            throw new NotImplementedException();
        }
    }
}