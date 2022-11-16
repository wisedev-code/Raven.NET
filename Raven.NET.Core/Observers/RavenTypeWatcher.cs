using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Exceptions;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Storage;
using Raven.NET.Core.Storage.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers
{
    /// <inheritdoc/>
    public class RavenTypeWatcher : IRavenTypeWatcher
    {
        private readonly IRavenProvider _ravenProvider;
        private readonly IRavenSettingsProvider _ravenSettingsProvider;
        
        private readonly IRavenStorage _ravenStorage = RavenStorage.Instance;

        private Func<RavenSubject,bool> _updateAction;
        private string _keyName;
        
        private ILogger<RavenTypeWatcher> _logger;
        private string RavenName;
        
        internal RavenSettings _ravenSettings = new();
        internal List<RavenSubject> _watchedSubjects = new();

        public RavenTypeWatcher(
            IRavenProvider ravenProvider,
            IRavenSettingsProvider ravenSettingsProvider)
        {
            _ravenProvider = ravenProvider;
            _ravenSettingsProvider = ravenSettingsProvider;
            
            _ravenSettings.BackgroundWorker = true;
            _ravenSettings.BackgroundWorkerInterval = 1.0f;
        }
        
        /// <inheritdoc/>
        void IRaven.Update(RavenSubject subject)
        {
            try
            {
                _updateAction(subject);
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
        public IRavenTypeWatcher Create<T>(string name, string keyName, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null) 
        {
            RavenName = name;
            
            _updateAction = callback;
            _ravenProvider.AddRaven(name,this, typeof(T));
            _ravenStorage.SubjectTypeTryAdd(typeof(T), new ConcurrentDictionary<string, string>());
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
            
            _logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(_ravenSettings.LogLevel);
            }).CreateLogger<RavenTypeWatcher>();

            if (options != null)
            {
                options(_ravenSettings);
            }

            if (_ravenSettings.BackgroundWorker.Value)
            {
                new System.Threading.Timer((e) 
                    => { BackgroundWorkerRun(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_ravenSettings.BackgroundWorkerInterval.Value));
            }
            
            _logger.LogInformation($"Created new raven. {RavenName}");
            _logger.LogDebug($"Raven log level: {_ravenSettings.LogLevel}, autoDestroy: {_ravenSettings.AutoDestroy}.");

            return this;
        }

        /// <inheritdoc/>
        public void Exclude(RavenSubject subject)
        {
            _watchedSubjects.Remove(subject);
            _logger.LogInformation($"Subject {subject.UniqueId} is excluded from {RavenName} raven watch list.");
        }
        
        /// <inheritdoc/>
        public void Stop(string name)
        {
            _logger.LogDebug($"Stopping raven {name}");
            var raven = _ravenProvider.GetRaven(name) as RavenTypeWatcher;
            _watchedSubjects.ForEach(x => x.Detach(this));
            _watchedSubjects.Clear();
            _ravenProvider.UpdateSubjects(raven.RavenName, _watchedSubjects);
            _logger.LogInformation($"Detached {raven._watchedSubjects.Count} subjects from raven {RavenName}");

            if (raven._ravenSettings.AutoDestroy)
            {
                TryDestroy(raven, name);
            }
        }

        /// <inheritdoc/>
        void IRavenTypeWatcher.UpdateNewestSubject(string key, RavenSubject subject)
        {
            _watchedSubjects.RemoveAll(x => x.GetType().GetProperty(_keyName)?.GetValue(x).ToString() == key);
            _logger.LogDebug($"Purged {RavenName} raven watch list.");
            _watchedSubjects.Add(subject);
            _logger.LogDebug($"Subject {subject.UniqueId} is added to {RavenName} raven watch list.");
        }
        
        /// <inheritdoc/>
        void IRavenTypeWatcher.AttachSubject(RavenSubject subject)
        {
            if (_watchedSubjects.Contains(subject))
            {
                throw new SubjectAlreadyWatchedException(subject.UniqueId.ToString());
            }
            
            _watchedSubjects.Add(subject);
            _logger.LogDebug($"Subject {subject.UniqueId} is added to {RavenName} raven watch list.");
        }

        /// <inheritdoc/>
        string IRavenTypeWatcher.KeyName => _keyName;
        
        private void BackgroundWorkerRun()
        {
            foreach (var subject in _watchedSubjects.ToList())
            {
                _logger.LogDebug($"Trying to notify subject {subject.UniqueId}");
                subject.TryNotify();
            }
        }
        
        private void TryDestroy(RavenTypeWatcher ravenWatcher, string ravenName)
        {
            _logger.LogDebug($"Trying to destroy raven {RavenName}");
            
            if (!ravenWatcher._watchedSubjects.Any())
            {
                _ravenStorage.RavenWatcherRemove(ravenName);
                _logger.LogInformation($"Removed raven {RavenName}");
                return;
            }
            
            _logger.LogWarning($"Raven {RavenName} is still watching some subjects, can't be destroyed");
        }
    }
}