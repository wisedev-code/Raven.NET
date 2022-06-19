using System;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers.Interfaces
{
    public interface IRavenWatcher : IRaven
    {
        public IRavenWatcher Create(string name, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null);
        public IRavenWatcher Watch(RavenSubject subject);
        public void UnWatch(string name, RavenSubject subject);
        public void Stop(string name);
    }
}