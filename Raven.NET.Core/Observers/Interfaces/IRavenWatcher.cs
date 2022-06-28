using System;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers.Interfaces
{
    /// <summary>
    /// Standard watcher to track changes based on single objects (in future also on arrays)
    /// </summary>
    public interface IRavenWatcher : IRaven
    {
        /// <summary>
        /// Method that initialize watcher
        /// </summary>
        /// <param name="name">name of watcher</param>
        /// <param name="callback">method to call when subject is updated</param>
        /// <param name="options">custom options, when not provided - default will be taken from configuration</param>
        /// <returns></returns>
        public IRavenWatcher Create(string name, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null);
        
        /// <summary>
        /// Attach subject to watcher
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public IRavenWatcher Watch(RavenSubject subject);
        
        /// <summary>
        /// Detach subject from watcher
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subject"></param>
        public void UnWatch(string name, RavenSubject subject);
        
        /// <summary>
        /// Detach all subjects from watcher
        /// </summary>
        /// <param name="name"></param>
        public void Stop(string name);
    }
}