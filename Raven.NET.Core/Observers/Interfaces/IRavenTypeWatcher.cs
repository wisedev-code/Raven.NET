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
        public IRavenWatcher Create<T>(string name, string keyName, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null);

        /// <summary>
        /// Ignore this subject from watching
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subject"></param>
        public void Exclude(string name, RavenSubject subject);
        
        /// <summary>
        /// Detach all subjects from watcher
        /// </summary>
        /// <param name="name"></param>
        public void Stop(string name);
    }
}