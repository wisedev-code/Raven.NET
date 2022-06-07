using System.Collections.Generic;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Static;

namespace Raven.NET.Core.Subjects
{
    public class RavenSubject
    {
        internal List<IRaven> Observers = new();
        public RavenSubject()
        {
            var clone = this.MemberwiseClone();
            RavenStore.SubjectStore.TryAdd(GetHashCode(), clone);
        }

        public void TryNotify()
        {
            var hashKey = GetHashCode();
            if (RavenStore.SubjectStore.ContainsKey(hashKey))
            {
                if (!RavenStore.SubjectStore[hashKey].Equals(this))
                {
                    RavenStore.SubjectStore[hashKey] = this;
                    //todo check list of observers and call update
                }
            }
        }
    }
}