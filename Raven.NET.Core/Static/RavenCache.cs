using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Static
{
    /// TODO This can be refactored nice way to use in memory cache
    internal static class RavenCache
    {
        public static ConcurrentDictionary<Guid, string> SubjectCache { get; } = new();

        public static ConcurrentDictionary<string, IRaven> RavenWatcherCache { get; } = new();

        public static ConcurrentDictionary<Type, IRaven> RavenTypeWatcherCache { get; } = new();
    }
}