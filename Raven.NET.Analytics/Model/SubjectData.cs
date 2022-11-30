using System;

namespace Raven.NET.Analytics.Model
{
    public class SubjectData
    {
        public string Id { get; set; }
        public string? RavenName { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string RawJson { get; set; }
    }
}