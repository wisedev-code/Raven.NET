using System;
using System.Collections.Generic;
using System.Text.Json;
using Raven.NET.Analytics.Model;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Analytics.Misc
{
    public static class RandomDataGenerator
    {
        public static List<RavenData> CreateRavens(int count)
        {
            List<RavenData> ravensData = new List<RavenData>();
            var rng = new Random();

            for (int i = 0; i < count; i++)
            {
                var watcherNumber = rng.NextInt64(1, 256);
                var subjectNumber = rng.NextInt64(1, 8000);
                var createdAtDiffMin = rng.NextInt64(-5000, 5000);
                var updatedAtDiffMin = rng.NextInt64(-100, -5);
                
                var raven = new RavenData()
                {
                    Name = $"SomeWatcher{watcherNumber}",
                    SubjectCount = subjectNumber,
                    CreatedAt = DateTime.Now.AddMinutes(createdAtDiffMin),
                    UpdatedAt = DateTime.Now.AddMinutes(updatedAtDiffMin),
                };
                
                ravensData.Add(raven);
            }

            return ravensData;
        }

        public static List<SubjectData> CreateSubjects(int count)
        {
            List<SubjectData> subjectData = new List<SubjectData>();
            var rng = new Random();
            
            for (int i = 0; i < count; i++)
            {
                var updatedAtDiffMin = rng.NextInt64(-100, -5);

                var dummyPhone = new DummyPhone()
                {
                    Id = Guid.NewGuid(),
                    Name = $"{rng.NextInt64(1, 555)}Phone",
                    Price = rng.NextInt64(100, 2137)
                };

                var ravenSubjectDto = new SubjectData()
                {
                    Id = dummyPhone.Id.ToString(),
                    RavenName = $"Raven{rng.NextInt64(1, 5)}",
                    RawJson = JsonSerializer.Serialize(dummyPhone),
                    UpdatedAt = DateTime.Now.AddMinutes(updatedAtDiffMin)
                };
                
                subjectData.Add(ravenSubjectDto);
            }

            return subjectData;
        }
    }

    public class DummyPhone : RavenSubject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}