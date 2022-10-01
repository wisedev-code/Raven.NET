using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Exceptions;
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
        private Func<RavenSubject,bool> updateAction;
        private ILogger<RavenWatcher> _logger;
        private string RavenName;

        internal RavenSettings _ravenSettings = new();
        internal List<RavenSubject> _watchedSubjects = new();


        public RavenWatcher(IRavenProvider ravenProvider, IRavenSettingsProvider ravenSettingsProvider)
        {
            _ravenProvider = ravenProvider;
            _ravenSettingsProvider = ravenSettingsProvider;
        }
        
        /// <inheritdoc/>
        void IRaven.Update(RavenSubject subject)
        {
            try
            {
                updateAction(subject);
                _logger.LogDebug($"Raven {RavenName} updated with subject {subject.UniqueId}.");
            }
            catch (Exception ex)
            {
                if (_ravenSettings.BreakOnUpdateException)
                {
                    throw new RavenUpdateCallbackException(RavenName, ex.Message);
                }
                
                _logger.LogError($"Raven: {RavenName} encounter error on running update callback ({ex.Message})");
            }
        }

        /// <inheritdoc/>
        public IRavenWatcher Create(string name, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null)
        {
            RavenName = name;

            var ravenSettings = _ravenSettingsProvider.GetRaven(name);

            if (ravenSettings != null)
            {
                _ravenSettings = ravenSettings;
            }

            _logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(_ravenSettings.LogLevel);
            }).CreateLogger<RavenWatcher>();
            
            
            updateAction = callback;
            _ravenProvider.AddRaven(name,this);

            if (options != null)
            {
                options(_ravenSettings);
            }
            
            _logger.LogInformation($"Created new raven. {RavenName}");
            _logger.LogDebug($"Raven log level: {_ravenSettings.LogLevel}, autoDestroy: {_ravenSettings.AutoDestroy}.");

            return this;
        }

        /// <inheritdoc/>
        public IRavenWatcher Watch(RavenSubject subject)
        {
            if (_watchedSubjects.Contains(subject))
            {
                throw new SubjectAlreadyWatchedException(subject.UniqueId.ToString());
            }
            
            subject.Attach(this);
            _watchedSubjects.Add(subject);
            _logger.LogInformation($"Subject {subject.UniqueId} is now watched by {RavenName}");
            return this;
        }

        /// <inheritdoc/>
        public void UnWatch(string name, RavenSubject subject)
        {
            var raven = _ravenProvider.GetRaven(name) as RavenWatcher;
            if (raven == null)
            {
                throw new RavenDoesNotExistsException(name);
            }
            _watchedSubjects.Remove(subject);
            _ravenProvider.UpdateSubjects(name, _watchedSubjects);
            _logger.LogInformation($"Subject {subject.UniqueId} is no longer watched by {RavenName}");

            if (!raven._ravenSettings.AutoDestroy)
                TryDestroy(raven, name);
        }
        
        /// <inheritdoc/>
        public void Stop(string name)
        {
            _logger.LogDebug($"Stopping raven {name}");
            var raven = _ravenProvider.GetRaven(name) as RavenWatcher;
            _watchedSubjects.ForEach(x => x.Detach(this));
            _watchedSubjects.Clear();
            _ravenProvider.UpdateSubjects(raven.RavenName, _watchedSubjects);
            _logger.LogInformation($"Detached {raven._watchedSubjects.Count} subjects from raven {RavenName}");

            if (raven._ravenSettings.AutoDestroy)
            {
                TryDestroy(raven, name);
            }
        }

        private void TryDestroy(RavenWatcher ravenWatcher, string ravenName)
        {
            _logger.LogDebug($"Trying to destroy raven {RavenName}");
            
            if (!ravenWatcher._watchedSubjects.Any())
            {
                RavenCache.RavenWatcherCache.Remove(ravenName, out _);
                _logger.LogInformation($"Removed raven {RavenName}");
                return;
            }
            
            _logger.LogWarning($"Raven {RavenName} is still watching some subjects, can't be destroyed");
        }
    }
}