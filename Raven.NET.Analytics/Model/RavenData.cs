using System;

namespace Raven.NET.Analytics.Model
{
    public class RavenData
    {
        public string Name { get; set; }
        public long SubjectCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}