using System.Collections.Concurrent;
using System.Net.Sockets;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Static
{
    internal static class RavenStore
    {
        public static ConcurrentDictionary<int, object> SubjectStore { get; set; } = new();

        public static ConcurrentDictionary<string, IRaven> RavenWatcherStore { get; set; } = new();
    }
}