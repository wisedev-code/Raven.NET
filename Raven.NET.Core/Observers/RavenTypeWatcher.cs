using System;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers
{
    /// <inheritdoc/>
    public class RavenTypeWatcher : IRavenTypeWatcher
    {
        /// <inheritdoc/>
        void IRaven.Update(RavenSubject subject)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IRavenWatcher Create<T>(string name, string keyName, Func<RavenSubject, bool> callback, Action<RavenSettings> options = null)
        {
            throw new NotImplementedException();
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