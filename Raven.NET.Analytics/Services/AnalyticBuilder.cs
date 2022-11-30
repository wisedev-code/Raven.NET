using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Raven.NET.Analytics.Model;
using Raven.NET.Core.Storage.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Raven.NET.Analytics.Services
{
    public class AnalyticBuilder
    {
        private readonly IRavenStorage _ravenStorage;
        
        public AnalyticBuilder(IRavenStorage ravenStorage)
        {
            _ravenStorage = ravenStorage;
        }
        
        public List<RavenData> BuildRavensData()
        {
            var ravens = _ravenStorage.GetAllRavens();
            var ravensDto = new List<RavenData>();

            foreach (var raven in ravens)
            {
                ravensDto.Add(new RavenData()
                {
                    UpdatedAt = raven.UpdatedAt,
                    CreatedAt = raven.CreatedAt,
                    Name = raven.Name,
                    SubjectCount = raven.SubjectCount
                });
            }

            return ravensDto;
        }

        public List<SubjectData> BuildSubjectsData()
        {
            var subjects = _ravenStorage.GetAllSubjects();

            var subjectDtos = new List<SubjectData>();
            foreach (var subject in subjects)
            {
                subjectDtos.Add(new SubjectData()
                {
                    Id = subject.Key,
                    RavenName = subject.Observers.FirstOrDefault()?.Name,
                    RawJson =  JsonConvert.SerializeObject(subject),
                    UpdatedAt = subject.UpdatedAt
                });
            }

            return subjectDtos;
        }
    }
}