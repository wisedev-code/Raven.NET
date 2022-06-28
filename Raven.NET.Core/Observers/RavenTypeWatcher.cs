using System;
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
    }
}