using System;

namespace Raven.NET.Core.Exceptions
{
    public class SubjectAlreadyWatchedException : RavenBaseException
    {
        public SubjectAlreadyWatchedException(string id) : base($"Subject with id: {id} is already watched by different Raven")
        {
            Code = "SUBJECT_ALREADY_WATCHED";
        }
    }
}