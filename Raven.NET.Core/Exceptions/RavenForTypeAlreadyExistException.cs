using System;

namespace Raven.NET.Core.Exceptions
{
    public class RavenForTypeAlreadyExistException : RavenBaseException
    {
        public RavenForTypeAlreadyExistException(Type type) : base($"Raven for type {type} is already registered")
        {
            Code = "RAVEN_ALREADY_REGISTERED";
        }
    }
}