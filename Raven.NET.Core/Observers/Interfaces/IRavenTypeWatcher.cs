using System;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers.Interfaces
{
    /// <summary>
    /// Watcher that can observe all objects of certain type
    /// </summary>
    public interface IRavenTypeWatcher : IRaven
    {
        /// <summary>
        /// Create watcher to track all objects of the same type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keyName"></param>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRavenTypeWatcher Create<T>(string name, string keyName, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null);

        /// <summary>
        /// Ignore this subject from watching
        /// </summary>
        /// <param name="subject"></param>
        public void Exclude(RavenSubject subject);
        
        /// <summary>
        /// Update subject list to contain only newest version
        /// </summary>
        /// <param name="subject"></param>
        internal void UpdateNewestSubject(string key, RavenSubject subject);

        /// <summary>
        /// Attach subject to the list of observable objects
        /// </summary>
        /// <param name="subject"></param>
        internal void AttachSubject(RavenSubject subject);
        
        internal string KeyName { get; }
        
        /// <summary>
        /// Detach all subjects from watcher
        /// </summary>
        /// <param name="name"></param>
        public void Stop(string name);
    }
}