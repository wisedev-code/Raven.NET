using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Observers.Interfaces
{
    /// <summary>
    /// Basic interface for watchers that will be forcing update implementation on each child interfaces
    /// </summary>
    public interface IRaven
    {
        /// <summary>
        /// Method that is being called on each subject update by notify or trynotify method
        /// </summary>
        /// <param name="subject"></param>
        internal void Update(RavenSubject subject);
    }
}